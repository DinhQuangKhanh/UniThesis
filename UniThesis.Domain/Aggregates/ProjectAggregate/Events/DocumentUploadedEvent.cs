using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Document;

namespace UniThesis.Domain.Aggregates.ProjectAggregate.Events
{
    public sealed record DocumentUploadedEvent(Guid ProjectId, Guid DocumentId, DocumentType DocumentType) : DomainEventBase;
}
