using Domain.SeedWork;

namespace Domain.Events.OrderEvents
{
    public record OrderPaidEvent(Guid OrderId) : IDomainEvent;
}
