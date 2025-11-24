using Domain.SeedWork;

namespace Domain.Entities
{
    // 2. DETAYLI LOKASYON (Depocu ürünü bulsun diye)
    public class InventoryLocation : Entity
    {
        public Guid TerminalInventoryId { get; private set; }
        public string Area { get; private set; }
        public string Corridor { get; private set; }
        public string Place { get; private set; } // Koridor A
        public string Shelf { get; private set; } // Raf 3
        public int Quantity { get; private set; }
    }
}