using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.Topics.DTOs;

namespace UniThesis.Application.Features.Topics.Queries.GetTopicDetail;

public class GetTopicDetailQueryHandler : IQueryHandler<GetTopicDetailQuery, TopicDetailDto?>
{
    private readonly ITopicQueryService _queryService;

    public GetTopicDetailQueryHandler(ITopicQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<TopicDetailDto?> Handle(GetTopicDetailQuery request, CancellationToken cancellationToken)
    {
        return await _queryService.GetTopicDetailAsync(request.TopicId, cancellationToken);
    }
}
