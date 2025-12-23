using Domain.SeedWork;

namespace Domain.Events.ProductEvents
{
    public record ProductUpdatedEvent(Guid ProductId, string Name) : IDomainEvent;
}
