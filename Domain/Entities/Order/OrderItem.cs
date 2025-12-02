using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Order
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }

        // --- Ürün (Hibrid) ---
        public Guid? PackageId { get; private set; } // Sistemde varsa
        public string NameSnapshot { get; private set; } // "6-Pack Kola" (İsim değişse de fatura bozulmaz)
        public CargoSpec SpecSnapshot { get; private set; } // Fiziksel özellikler

        // --- Miktar ve Fiyat ---
        public int Quantity { get; private set; }
        public Money UnitPriceSnapshot { get; private set; } // *** Snapshot *** (Zamlardan etkilenmez)

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid? packageId, string name, CargoSpec spec, int quantity, Money unitPrice)
        {
            OrderId = orderId;
            PackageId = packageId;
            NameSnapshot = name;
            SpecSnapshot = spec;
            Quantity = quantity;
            UnitPriceSnapshot = unitPrice;
        }
    }
}