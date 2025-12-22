using Domain.SeedWork;

namespace Domain.Events.RegisterEvents
{
    public record AppUserCreatedEvent(Guid UserId, string Email) : IDomainEvent;
}
