using Domain.Entities.Order;
using Domain.SeedWork;
using MediatR;

namespace Domain.Events.OrderEvents
{
    public class OrderPaidEvent : IDomainEvent
    {
        public Order Order { get; }

        public OrderPaidEvent(Order order)
        {
            Order = order;
        }
    }
}
