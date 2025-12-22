using Domain.SeedWork;

namespace Domain.Events.RegisterEvents
{
    public record IndividualCustomerRegisteredEvent(Guid CustomerId, string Name, string Email) : IDomainEvent;
}
