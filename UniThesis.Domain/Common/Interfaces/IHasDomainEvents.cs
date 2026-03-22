using System.Collections.Generic;

namespace UniThesis.Domain.Common.Interfaces
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
