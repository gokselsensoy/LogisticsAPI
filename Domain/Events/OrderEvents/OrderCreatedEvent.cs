using Domain.SeedWork;

namespace Domain.Events.OrderEvents
{
    public record OrderCreatedEvent(Guid OrderId, Guid SupplierId, Guid? CustomerId) : IDomainEvent;
}
