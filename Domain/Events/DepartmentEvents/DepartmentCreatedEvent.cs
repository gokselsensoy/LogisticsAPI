using Domain.SeedWork;

namespace Domain.Events.DepartmentEvents
{
    public record DepartmentCreatedEvent(Guid DepartmentId, Guid CompanyId, string Name) : IDomainEvent;
}
