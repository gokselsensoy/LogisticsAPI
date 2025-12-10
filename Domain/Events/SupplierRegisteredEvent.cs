using Domain.SeedWork;

namespace Domain.Events
{
    public record SupplierRegisteredEvent(Guid CompanyId, string Name, string Email) : IDomainEvent;
}
