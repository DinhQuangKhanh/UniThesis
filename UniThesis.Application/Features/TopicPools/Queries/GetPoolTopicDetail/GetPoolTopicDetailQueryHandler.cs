using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetPoolTopicDetail;

public class GetPoolTopicDetailQueryHandler : IQueryHandler<GetPoolTopicDetailQuery, PoolTopicDetailDto?>
{
    private readonly ITopicPoolQueryService _queryService;

    public GetPoolTopicDetailQueryHandler(ITopicPoolQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<PoolTopicDetailDto?> Handle(GetPoolTopicDetailQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetPoolTopicDetailAsync(request.ProjectId, cancellationToken);
    }
}
