using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }

        // --- Ürün (Snapshot) ---
        public Guid PackageId { get; private set; } // Hangi ürün satıldı?
        public string NameSnapshot { get; private set; }
        public CargoSpec SpecSnapshot { get; private set; }

        // --- Miktar ve Fiyat ---
        public int Quantity { get; private set; }
        public Money UnitPriceSnapshot { get; private set; }

        public int ReturnedQuantity { get; private set; }

        // --- Lojistik Durumu ---
        public int ShippedQuantity { get; private set; } // Kaç tanesi kargoya verildi?

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid packageId, string name, CargoSpec spec, int quantity, Money unitPrice)
        {
            OrderId = orderId;
            PackageId = packageId;
            NameSnapshot = name;
            SpecSnapshot = spec;
            Quantity = quantity;
            UnitPriceSnapshot = unitPrice;
            ShippedQuantity = 0;
            ReturnedQuantity = 0;
        }

        public void IncreaseShippedQuantity(int amount)
        {
            if (ShippedQuantity + amount > Quantity)
                throw new DomainException("Sipariş edilenden fazlası gönderilemez.");

            ShippedQuantity += amount;
        }

        public void IncreaseReturnedQuantity(int amount)
        {
            if (ReturnedQuantity + amount > Quantity)
                throw new DomainException("Satılan adetten fazlası iade edilemez.");

            ReturnedQuantity += amount;
        }
    }
}