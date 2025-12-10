using Domain.SeedWork;

namespace Domain.Events.DepartmentEvents
{
    public record DepartmentUpdatedEvent(Guid DepartmentId, Guid CompanyId) : IDomainEvent;
}
