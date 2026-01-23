using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Domain.Common.Primitives
{
    public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents
            => _domainEvents.AsReadOnly();

        protected AggregateRoot(TId id) : base(id) { }

        protected AggregateRoot() { }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
