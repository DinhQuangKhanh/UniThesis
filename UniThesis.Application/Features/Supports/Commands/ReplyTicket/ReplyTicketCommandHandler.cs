using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.SupportAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Application.Features.Supports.Commands.ReplyTicket;

public class ReplyTicketCommandHandler : ICommandHandler<ReplyTicketCommand>
{
    private readonly ISupportTicketRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ReplyTicketCommandHandler(ISupportTicketRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ReplyTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(SupportTicket), request.TicketId);

        ticket.AddMessage(request.SenderId, request.Content);

        // Optional: If Admin replies to an Open ticket, auto transition to InProgress
        // But since we aren't passing Role in the command, we'll leave that logic for the API layer or specific admin command.
        
        _repository.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
