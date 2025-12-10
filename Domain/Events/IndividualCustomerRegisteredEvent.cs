using Domain.SeedWork;

namespace Domain.Events
{
    public record IndividualCustomerRegisteredEvent(Guid CustomerId, string Name, string Email) : IDomainEvent;
}
