using Domain.SeedWork;

namespace Domain.Events
{
    public record AppUserCreatedEvent(Guid UserId, string Email) : IDomainEvent;
}
