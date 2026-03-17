using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Aggregates.SupportAggregate.ValueObjects;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Application.Features.Supports.Commands.CreateTicket;

public class CreateTicketCommandHandler : ICommandHandler<CreateTicketCommand, Guid>
{
    private readonly ISupportTicketRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        // Generate Code TK-YYYY-SEQ
        var year = DateTime.UtcNow.Year;
        var seq = await _repository.GetNextSequenceAsync(year, cancellationToken);
        var code = TicketCode.Generate(year, seq);

        var ticket = SupportTicket.Create(
            code,
            request.Title,
            request.Description,
            request.ReporterId,
            request.Category,
            request.Priority);

        await _repository.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}
