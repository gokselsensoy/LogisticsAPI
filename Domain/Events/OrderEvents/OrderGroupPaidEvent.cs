using Domain.Entities.Orders;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;

namespace Domain.Events.OrderEvents
{
    public class OrderGroupPaidEvent : IDomainEvent
    {
        public Guid OrderGroupId { get; }
        public string OrderNumber { get; }
        public Guid CustomerId { get; }
        public Money TotalAmount { get; }
        public DateTime PaidAt { get; }

        // Constructor
        public OrderGroupPaidEvent(Guid orderGroupId, string orderNumber, Guid customerId, Money totalAmount)
        {
            OrderGroupId = orderGroupId;
            OrderNumber = orderNumber;
            CustomerId = customerId;
            TotalAmount = totalAmount;
            PaidAt = DateTime.UtcNow;
        }
    }
}
