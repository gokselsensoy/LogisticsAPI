using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Inventory
{
    public class InventoryTransaction : Entity, IAggregateRoot
    {
        public Guid InventoryId { get; private set; } // İşlem nerede oldu?
        public Guid PackageId { get; private set; }   // Hangi ürün?
        public Guid OwnerId { get; private set; }     // Kimin malı?

        public int QuantityChange { get; private set; } // +50 veya -10
        public int QuantityAfter { get; private set; }  // İşlemden sonra rafta kaç kaldı? (Snapshot)

        public TransactionType Type { get; private set; } // Giriş, Çıkış, Sayım Farkı, Transfer

        public Guid WorkerId { get; private set; } // Kim yaptı? (WorkerId veya AppUserId)
        public DateTime CreatedAt { get; private set; } // Ne zaman?
        public string? Note { get; private set; } // "Admin manuel ekledi", "Sipariş #123 için"

        private InventoryTransaction() { }

        public InventoryTransaction(Guid inventoryId, Guid packageId, Guid ownerId, int quantityChange, int quantityAfter, TransactionType type, Guid workerId, string? note)
        {
            Id = Guid.NewGuid();
            InventoryId = inventoryId;
            PackageId = packageId;
            OwnerId = ownerId;
            QuantityChange = quantityChange;
            QuantityAfter = quantityAfter;
            Type = type;
            WorkerId = workerId;
            Note = note;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

//public async Task<List<InventorySummaryDto>> GetTerminalSummaryAsync(Guid terminalId)
//{
//    // SQL Karşılığı: 
//    // SELECT PackageId, SUM(Quantity) FROM Stocks 
//    // WHERE LocationId IN (Select Id From Locations Where TerminalId = ...)
//    // GROUP BY PackageId

//    return await _context.Stocks
//        .Where(s => s.Location.TerminalId == terminalId) // Navigation Property ile erişim
//        .GroupBy(s => new { s.PackageId, s.State }) // Ürün ve Duruma göre grupla
//        .Select(g => new InventorySummaryDto
//        {
//            PackageId = g.Key.PackageId,
//            State = g.Key.State.ToString(),
//            TotalQuantity = g.Sum(x => x.Quantity) // ANLIK HESAPLAMA
//        })
//        .ToListAsync();
//}