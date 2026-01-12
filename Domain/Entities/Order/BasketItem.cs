using Domain.SeedWork;

namespace Domain.Entities.Orders
{
    public class BasketItem : Entity
    {
        public Guid BasketId { get; private set; }
        public Guid PackageId { get; private set; }
        public Guid SupplierId { get; private set; } // Siparişi kime oluşturacağız?
        public int Quantity { get; private set; }

        private BasketItem() { }

        public BasketItem(Guid basketId, Guid packageId, Guid supplierId, int quantity)
        {
            BasketId = basketId;
            PackageId = packageId;
            SupplierId = supplierId;
            Quantity = quantity;
        }

        internal void IncreaseQuantity(int amount) => Quantity += amount;
    }
}