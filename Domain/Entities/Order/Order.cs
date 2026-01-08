using Domain.Enums;
using Domain.Events.OrderEvents;
using Domain.Events.WorkerEvents;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Order
{
    public class Order : FullAuditedEntity, IAggregateRoot
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

        public void AddItem(Guid packageId, string name, CargoSpec spec, int quantity, Money unitPrice)
        {
            // --- DÜZELTME BURADA ---

            // 1. Eğer sipariş listesi boşsa (yani bu ilk ürünse),
            // Siparişin TotalPrice'ını, gelen ürünün para birimine göre güncelle (resetle).
            if (!_items.Any())
            {
                // Miktar henüz 0 ama para birimi artık ürünün para birimi (örn: USD)
                TotalPrice = new Money(0, unitPrice.Currency);
            }

            // 2. Validation: Artık para birimleri eşitlenmiş olmalı.
            // Eğer 2. ürünü ekliyorsak ve farklı para birimindeyse o zaman hata vermeli.
            if (unitPrice.Currency != TotalPrice.Currency)
                throw new DomainException($"Sipariş para birimi ({TotalPrice.Currency}) ile ürün para birimi ({unitPrice.Currency}) uyuşmuyor.");

            // -----------------------

            var item = new OrderItem(Id, packageId, name, spec, quantity, unitPrice);
            _items.Add(item);

            RecalculateTotal();
        }

        public void ConfirmOrder()
        {
            if (!_items.Any()) throw new DomainException("Boş sipariş onaylanamaz.");
            Status = OrderStatus.Confirmed; // Veya direkt Confirmed
            AddDomainEvent(new OrderCreatedEvent(Id, SupplierId, CustomerId));
        }

        public void MarkAsPaid()
        {
            Status = OrderStatus.Processing; // İşleniyor (Depoya düştü)
            AddDomainEvent(new OrderPaidEvent(this));
        }

        private void RecalculateTotal()
        {
            decimal totalAmount = _items.Sum(i => i.Quantity * i.UnitPriceSnapshot.Amount);
            TotalPrice = new Money(totalAmount, TotalPrice.Currency);
        }
    }
}