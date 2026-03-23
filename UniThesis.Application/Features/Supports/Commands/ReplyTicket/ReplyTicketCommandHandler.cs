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

        // No need for _repository.Update(ticket) — entity is already tracked by EF Core.
        // Calling Update() would force all properties to Modified state, interfering
        // with proper detection of the new TicketMessage as an Added entity.
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
