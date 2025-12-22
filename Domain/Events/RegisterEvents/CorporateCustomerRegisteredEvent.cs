using Domain.SeedWork;

namespace Domain.Events.RegisterEvents
{
    public record CorporateCustomerRegisteredEvent(Guid CustomerId, string CompanyName, string Email) : IDomainEvent;
}
