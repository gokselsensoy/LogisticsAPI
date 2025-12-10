using Domain.SeedWork;

namespace Domain.Events.DepartmentEvents
{
    public record DepartmentDeletedEvent(Guid DepartmentId, Guid CompanyId) : IDomainEvent;
}
