using Domain.SeedWork;

namespace Domain.Entities
{
    public class TerminalInventory : Entity
    {
        public Guid TerminalId { get; private set; }
        public Guid PackageId { get; private set; }
        public int TotalQuantity { get; private set; } // Toplam fiziksel stok
        public int ReservedQuantity { get; private set; } // Satılmış ama çıkmamış

        // Transporter deposunda başkasının malı durabilir (OwnerId)
        public Guid OwnerId { get; private set; } // Malın sahibi kim? (SupplierId)
    }
}