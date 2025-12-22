using Domain.SeedWork;

namespace Domain.Events.RegisterEvents
{
    public record FreelancerRegisteredEvent(Guid FreelancerId, string FullName, string Phone, string CvrNumber, string Email) : IDomainEvent;
}
