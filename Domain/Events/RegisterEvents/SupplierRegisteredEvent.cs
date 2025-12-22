using Domain.SeedWork;

namespace Domain.Events.RegisterEvents
{
    public record SupplierRegisteredEvent(Guid CompanyId, string Name, string Email) : IDomainEvent;
}
