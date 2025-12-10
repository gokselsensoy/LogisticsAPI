using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Order
{
    public class Order : Entity, IAggregateRoot
    {
        // --- Kaynak ---
        public OrderOrigin Origin { get; private set; }
        public string? ExternalReferenceCode { get; private set; } // Dış sistemin ID'si (Varsa)

        // --- Müşteri (Hibrid) ---
        public Guid? CustomerId { get; private set; } // Sistemde kayıtlıysa
        public ContactInfo Contact { get; private set; } // Kayıtsızsa (API/CSV)

        // --- Adres (Snapshot + Link) ---
        public Guid? CustomerAddressId { get; private set; } // Raporlama için ID referansı
        public Address DeliveryAddressSnapshot { get; private set; } // *** Snapshot *** (Adres değişse bile bu sabit kalır)

        // --- Finans ---
        public Guid SupplierId { get; private set; }
        public PaymentContext PaymentInfo { get; private set; } // Ödeme ve Mutabakat kuralları
        public Money TotalPrice { get; private set; } // *** Snapshot *** (Sipariş anındaki toplam)

        public OrderStatus Status { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { }

        public Order(
            OrderOrigin origin,
            Guid supplierId,
            PaymentContext paymentInfo,
            Address deliveryAddress,
            ContactInfo contact,
            Guid? customerId = null,
            Guid? addressId = null)
        {
            Id = Guid.NewGuid();
            Origin = origin;
            SupplierId = supplierId;
            PaymentInfo = paymentInfo;
            DeliveryAddressSnapshot = deliveryAddress;
            Contact = contact;
            CustomerId = customerId;
            CustomerAddressId = addressId;
            Status = OrderStatus.Draft;
        }

        public void AddItem(OrderItem item)
        {
            _items.Add(item);
            // Toplam fiyatı güncelleme mantığı buraya...
        }
    }
}