using MediatR;
using Microsoft.Extensions.Logging;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;

namespace UniThesis.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that invalidates cache entries after a command succeeds.
/// Only runs for commands implementing ICacheInvalidatingCommand.
/// Clears both L1 and L2 cache for all specified prefixes.
/// </summary>
public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatingCommand
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationBehavior(
        ICacheService cacheService,
        ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        var requestName = typeof(TRequest).Name;
        var prefixes = request.CachePrefixesToInvalidate;

        foreach (var prefix in prefixes)
        {
            await _cacheService.RemoveByPrefixAsync(prefix, cancellationToken);
            _logger.LogInformation(
                "Cache invalidated by {RequestName}: prefix {Prefix}",
                requestName, prefix);
        }

        return response;
    }
}

/// <summary>
/// MediatR pipeline behavior for commands that return a result and also invalidate cache.
/// Handles commands implementing ICacheInvalidatingCommand&lt;TResponse&gt;.
/// </summary>
public class CacheInvalidationWithResultBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatingCommand<TResponse>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheInvalidationWithResultBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationWithResultBehavior(
        ICacheService cacheService,
        ILogger<CacheInvalidationWithResultBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        var requestName = typeof(TRequest).Name;
        var prefixes = request.CachePrefixesToInvalidate;

        foreach (var prefix in prefixes)
        {
            await _cacheService.RemoveByPrefixAsync(prefix, cancellationToken);
            _logger.LogInformation(
                "Cache invalidated by {RequestName}: prefix {Prefix}",
                requestName, prefix);
        }

        return response;
    }
}
