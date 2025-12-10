using Domain.SeedWork;

namespace Domain.Events
{
    public record FreelancerRegisteredEvent(Guid FreelancerId, string Name, string Email) : IDomainEvent;
}
