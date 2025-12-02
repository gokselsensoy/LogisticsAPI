using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Inventory
{
    public class Package : Entity
    {
        public Guid ProductId { get; private set; }
        public string Name { get; private set; } // "6-Pack"
        public PackageType Type { get; private set; } // Pallet, Parcel, Piece
        public Dimensions Dimensions { get; private set; }

        // Hiyerarşi: 1 Palet = 50 Koli ise, AtomicQuantity = 50 (veya içindeki en küçük birim sayısı)
        public int AtomicQuantity { get; private set; }
        public int BoxQuantity { get; private set; }

        public bool IsReturnable { get; private set; }
        public Money DepositPrice { get; private set; }

        private Package() { }

        public Package(Guid productId, string name, PackageType type, int atomicQuantity)
        {
            ProductId = productId;
            Name = name;
            Type = type;
            AtomicQuantity = atomicQuantity;
        }
    }
}