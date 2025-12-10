using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Inventory
{
    public class Stock : Entity
    {
        public Guid InventoryId { get; private set; }
        public Guid PackageId { get; private set; }
        public int Quantity { get; private set; }
        public Guid OwnerId { get; private set; }

        public InventoryState State { get; private set; }

        private Stock() { }

        public Stock(Guid inventoryId, Guid packageId, int quantity, Guid ownerId, InventoryState state)
        {
            InventoryId = inventoryId;
            PackageId = packageId;
            Quantity = quantity;
            OwnerId = ownerId;
            State = state;
        }

        internal void Increase(int amount) => Quantity += amount;
        internal void Decrease(int amount) => Quantity -= amount;
    }
}