using Domain.SeedWork;

namespace Domain.Events.ProductEvents
{
    public record ProductCreatedEvent(Guid ProductId, Guid SupplierId, string Name) : IDomainEvent;
}
