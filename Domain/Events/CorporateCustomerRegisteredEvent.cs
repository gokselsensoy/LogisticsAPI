using Domain.SeedWork;

namespace Domain.Events
{
    public record CorporateCustomerRegisteredEvent(Guid CustomerId, string CompanyName, string Email) : IDomainEvent;
}
