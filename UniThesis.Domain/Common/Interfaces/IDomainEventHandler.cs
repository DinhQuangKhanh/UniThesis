// This file is deprecated. Use MediatR's INotificationHandler<T> instead.
// Kept for reference during migration.
// TODO: Remove this file after all handlers are migrated.

/*
namespace UniThesis.Domain.Common.Interfaces
{
    [Obsolete("Use MediatR.INotificationHandler<T> instead")]
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
*/
