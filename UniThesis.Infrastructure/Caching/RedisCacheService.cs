using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace UniThesis.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _settings;
        private readonly ILogger<RedisCacheService> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public RedisCacheService(IDistributedCache cache, IOptions<CacheSettings> settings, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var data = await _cache.GetStringAsync(key, ct);
            if (string.IsNullOrEmpty(data)) return default;

            try
            {
                return JsonSerializer.Deserialize<T>(data, JsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize cache value for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            var data = JsonSerializer.Serialize(value, JsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes)
            };

            await _cache.SetStringAsync(key, data, options, ct);
            _logger.LogDebug("Redis cache set: {Key}", key);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            await _cache.RemoveAsync(key, ct);
            _logger.LogDebug("Redis cache removed: {Key}", key);
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
        {
            // Note: This requires Redis SCAN command which is not available in IDistributedCache
            // In production, use StackExchange.Redis directly for this operation
            _logger.LogWarning("RemoveByPrefixAsync is not fully supported with IDistributedCache");
            return Task.CompletedTask;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            var cached = await GetAsync<T>(key, ct);
            if (cached is not null) return cached;

            var value = await factory();
            await SetAsync(key, value, expiration, ct);
            return value;
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            var data = await _cache.GetStringAsync(key, ct);
            return !string.IsNullOrEmpty(data);
        }
    }
}
