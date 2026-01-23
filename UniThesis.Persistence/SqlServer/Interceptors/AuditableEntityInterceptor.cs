using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Common.Primitives;

namespace UniThesis.Persistence.SqlServer.Interceptors
{
    /// <summary>
    /// Interceptor for automatically setting audit fields on entities.
    /// </summary>
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public AuditableEntityInterceptor(
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context is null) return;

            var utcNow = _dateTimeService.UtcNow;
            var userId = _currentUserService.UserId;

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity<Guid>>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(userId);
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(userId);
                }
            }

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity<int>>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetCreated(userId);
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetUpdated(userId);
                }
            }
        }
    }
}
