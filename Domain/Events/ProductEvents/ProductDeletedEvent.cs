using Domain.SeedWork;

namespace Domain.Events.ProductEvents
{
    public record ProductDeletedEvent(Guid ProductId) : IDomainEvent;
}
