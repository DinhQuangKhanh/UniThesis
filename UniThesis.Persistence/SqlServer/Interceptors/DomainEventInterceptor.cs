using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Common.Primitives;

namespace UniThesis.Persistence.SqlServer.Interceptors
{
    /// <summary>
    /// Interceptor for dispatching domain events after save.
    /// </summary>
    public class DomainEventInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override int SavedChanges(
            SaveChangesCompletedEventData eventData,
            int result)
        {
            DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
            return base.SavedChanges(eventData, result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents(eventData.Context);
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private async Task DispatchDomainEvents(DbContext? context)
        {
            if (context is null) return;

            var aggregateRoots = context.ChangeTracker
                .Entries<AggregateRoot<Guid>>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = aggregateRoots
                .SelectMany(ar => ar.DomainEvents)
                .ToList();

            aggregateRoots.ForEach(ar => ar.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                // Get handler type dynamically
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

                // Resolve all handlers for this event type
                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    if (handler is null) continue;

                    var method = handlerType.GetMethod("HandleAsync");
                    if (method is not null)
                    {
                        var task = method.Invoke(handler, new object[] { domainEvent, CancellationToken.None }) as Task;
                        if (task is not null)
                        {
                            await task;
                        }
                    }
                }
            }
        }
    }
}
