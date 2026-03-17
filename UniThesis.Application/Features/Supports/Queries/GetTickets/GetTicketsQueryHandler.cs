using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Supports.DTOs;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.UserAggregate;

namespace UniThesis.Application.Features.Supports.Queries.GetTickets;

public class GetTicketsQueryHandler : IQueryHandler<GetTicketsQuery, List<TicketListDto>>
{
    private readonly ISupportTicketRepository _supportTicketRepository;
    private readonly IUserRepository _userRepository;

    public GetTicketsQueryHandler(ISupportTicketRepository supportTicketRepository, IUserRepository userRepository)
    {
        _supportTicketRepository = supportTicketRepository;
        _userRepository = userRepository;
    }

    public async Task<List<TicketListDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<SupportTicket> tempTickets;

        // Since ISupportTicketRepository doesn't expose an IQueryable or GetAll with filters,
        // we use the closest method and filter in memory. For a real prod app with massive tickets,
        // it's better to add a specific method to ISupportTicketRepository.
        if (request.Status.HasValue)
        {
            tempTickets = await _supportTicketRepository.GetByStatusAsync(request.Status.Value, cancellationToken);
        }
        else
        {
            // Default to grabbing all (we can achieve this by fetching all statuses or adding GetAll to IRepository, which it has).
            tempTickets = await _supportTicketRepository.GetAllAsync(cancellationToken);
        }

        if (request.Priority.HasValue)
        {
            tempTickets = tempTickets.Where(t => t.Priority == request.Priority.Value);
        }

        var tickets = tempTickets.OrderByDescending(t => t.CreatedAt).ToList();

        // Fetch users (Reporters)
        var reporterIds = tickets.Select(t => t.ReporterId).Distinct().ToList();
        var usersList = await _userRepository.GetByIdsAsync(reporterIds, cancellationToken);
        var users = usersList.ToDictionary(u => u.Id, u => new UserBriefDto(u.Id, u.FullName, u.Email, string.Join(", ", u.GetActiveRoles())));

        var result = tickets.Select(t => new TicketListDto
        {
            Id = t.Id,
            Code = t.Code.Value,
            Title = t.Title,
            Category = t.Category.ToString(),
            Priority = t.Priority.ToString(),
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt,
            Reporter = users.GetValueOrDefault(t.ReporterId)!
        });

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLowerInvariant();
            result = result.Where(t => 
                t.Code.ToLowerInvariant().Contains(search) || 
                t.Title.ToLowerInvariant().Contains(search) ||
                (t.Reporter?.FullName?.ToLowerInvariant().Contains(search) ?? false));
        }

        return result.ToList();
    }
}
