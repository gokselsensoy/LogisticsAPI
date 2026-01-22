using Domain.Enums;
using Domain.Events.OrderEvents;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders
{
    public class OrderGroup : FullAuditedEntity, IAggregateRoot
    {
        public string OrderNumber { get; private set; } // Kullanıcı Dostu No: #202405-XA
        public Guid CustomerId { get; private set; }

        // Müşteriden Çekilen TOPLAM Tutar (Tüm tedarikçiler dahil)
        public Money TotalPrice { get; private set; }

        public PaymentContext PaymentInfo { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; }

        // Alt Siparişler (Tedarikçi Bazlı)
        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private OrderGroup() { }

        public OrderGroup(Guid customerId, string orderNumber, PaymentContext paymentInfo)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            OrderNumber = orderNumber;
            PaymentInfo = paymentInfo;
            PaymentStatus = PaymentStatus.Pending;
            TotalPrice = new Money(0, "TRY"); // Default, siparişler eklendikçe artacak
        }

        // Sepetten gelen her bir tedarikçi grubu için bir Order eklenir
        public void AddOrder(Order order)
        {
            // Validation: Para birimleri tutuyor mu?
            if (_orders.Any() && order.TotalPrice.Currency != TotalPrice.Currency)
                throw new DomainException("Farklı para birimindeki siparişler aynı grupta olamaz.");

            _orders.Add(order);
            RecalculateGroupTotal();
        }

        public void MarkAsPaid()
        {
            PaymentStatus = PaymentStatus.Paid;

            // PaidEventHandlerda kullanılacak.
            // Event'i fırlatırken gerekli dataları içine koyuyoruz
            AddDomainEvent(new OrderGroupPaidEvent(
                Id,
                OrderNumber,
                CustomerId,
                TotalPrice
            ));
        }

        private void RecalculateGroupTotal()
        {
            // Alt siparişlerin toplamı ana toplamı oluşturur
            var totalAmount = _orders.Sum(o => o.TotalPrice.Amount);
            var currency = _orders.FirstOrDefault()?.TotalPrice.Currency ?? "TRY";
            TotalPrice = new Money(totalAmount, currency);
        }
    }
}
