using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Application.Features.TopicPools.DTOs;

namespace UniThesis.Application.Features.TopicPools.Queries.GetTopicPoolById;

public class GetTopicPoolByIdQueryHandler : IQueryHandler<GetTopicPoolByIdQuery, TopicPoolDto>
{
    private readonly ITopicPoolQueryService _queryService;

    public GetTopicPoolByIdQueryHandler(ITopicPoolQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<TopicPoolDto> Handle(GetTopicPoolByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _queryService.GetTopicPoolByIdAsync(request.Id, cancellationToken);
        if (result == null)
            throw new Exception($"TopicPool with ID {request.Id} not found.");
            
        return result;
    }
}
