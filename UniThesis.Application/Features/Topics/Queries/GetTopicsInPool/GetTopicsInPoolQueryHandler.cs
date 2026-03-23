using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Topics.DTOs;

namespace UniThesis.Application.Features.Topics.Queries.GetTopicsInPool;

public class GetTopicsInPoolQueryHandler : IQueryHandler<GetTopicsInPoolQuery, GetTopicsInPoolResult>
{
    private readonly ITopicQueryService _queryService;

    public GetTopicsInPoolQueryHandler(ITopicQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<GetTopicsInPoolResult> Handle(GetTopicsInPoolQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetTopicsInPoolAsync(
            request.MajorId,
            request.Search,
            request.PoolStatus,
            request.SortBy,
            request.Page,
            request.PageSize,
            cancellationToken);
    }
}
