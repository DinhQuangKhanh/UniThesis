using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Ticket;

namespace UniThesis.Application.Features.Supports.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommandHandler : ICommandHandler<UpdateTicketStatusCommand>
{
    private readonly ISupportTicketRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketStatusCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(SupportTicket), request.TicketId);

        // Map request to Domain methods to enforce rules and fire events
        switch (request.Status)
        {
            case TicketStatus.InProgress:
                if (ticket.Status == TicketStatus.Open) ticket.StartProgress();
                break;
            case TicketStatus.Resolved:
                ticket.Resolve();
                break;
            case TicketStatus.Closed:
                ticket.Close();
                break;
            case TicketStatus.Open:
                if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Resolved)
                    ticket.Reopen();
                break;
        }

        _repository.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
