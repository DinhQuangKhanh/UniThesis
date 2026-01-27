namespace UniThesis.Infrastructure.Caching
{
    public class CacheSettings
    {
        public const string SectionName = "CacheSettings";
        public string Provider { get; set; } = "Memory"; // Memory, Redis
        public int DefaultExpirationMinutes { get; set; } = 30;
        public string? RedisConnectionString { get; set; }
    }
}
