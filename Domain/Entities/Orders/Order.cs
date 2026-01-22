using Domain.Enums;
using Domain.Events.OrderEvents;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders
{
    public class Order : FullAuditedEntity, IAggregateRoot
    {
        public Guid OrderGroupId { get; internal set; } // Master Siparişe Bağlantı

        // --- KİMLİK (Denormalizasyon İçerir) ---
        public Guid SupplierId { get; private set; }
        public Guid CustomerId { get; private set; } // **PERFORMANS**: Supplier müşteriyi hızlı bulsun diye.

        // --- DETAYLAR ---
        public OrderOrigin Origin { get; private set; }
        public string? ExternalReferenceCode { get; private set; }
        public ContactInfo Contact { get; private set; }

        // Adresler (Snapshot)
        public Guid? CustomerAddressId { get; private set; }
        public Address DeliveryAddressSnapshot { get; private set; }

        // --- FİNANS ---
        // Tedarikçiye ödenecek tutar (Ürün bedelleri toplamı)
        public Money TotalPrice { get; private set; }
        public Money RefundedAmount { get; private set; }

        // Platformun/Multillo'nun keseceği komisyon (Hesaplamalar için)
        public Money CommissionAmount { get; private set; }

        public OrderStatus Status { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; } // Bu siparişin parası ödendi mi?

        // İade Detayları
        public bool IsRefunded { get; private set; }
        public string? RefundReason { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { }

        public Order(
            Guid orderGroupId,
            OrderOrigin origin,
            Guid supplierId,
            Guid customerId, // Denormalized field
            Address deliveryAddress,
            ContactInfo contact,
            Guid? addressId = null,
            string? externalRef = null)
        {
            Id = Guid.NewGuid();
            OrderGroupId = orderGroupId;
            Origin = origin;
            SupplierId = supplierId;
            CustomerId = customerId;
            DeliveryAddressSnapshot = deliveryAddress;
            Contact = contact;
            CustomerAddressId = addressId;
            ExternalReferenceCode = externalRef;

            Status = OrderStatus.Draft;
            PaymentStatus = PaymentStatus.Pending;
            TotalPrice = new Money(0, "DK");
            CommissionAmount = new Money(0, "DK");
            RefundedAmount = new Money(0, "DK");

            IsRefunded = false;
        }

        public void AddItem(Guid packageId, string name, CargoSpec spec, int quantity, Money unitPrice)
        {
            if (!_items.Any())
            {
                TotalPrice = new Money(0, unitPrice.Currency);
                CommissionAmount = new Money(0, unitPrice.Currency);
            }

            if (unitPrice.Currency != TotalPrice.Currency)
                throw new DomainException("Para birimi uyuşmazlığı.");

            var item = new OrderItem(Id, packageId, name, spec, quantity, unitPrice);
            _items.Add(item);

            RecalculateTotal();
        }

        public void Confirm()
        {
            Status = OrderStatus.Confirmed;
            // Supplier'a bildirim eventi
            AddDomainEvent(new OrderCreatedEvent(Id, SupplierId, CustomerId));
        }

        public void SetCommission(decimal amount)
        {
            CommissionAmount = new Money(amount, TotalPrice.Currency);
        }

        public void MarkAsPaid()
        {
            if (PaymentStatus == PaymentStatus.Paid) return;
            PaymentStatus = PaymentStatus.Paid;
        }

        private void RecalculateTotal()
        {
            var total = _items.Sum(i => i.Quantity * i.UnitPriceSnapshot.Amount);
            TotalPrice = new Money(total, TotalPrice.Currency);
        }
    }
}