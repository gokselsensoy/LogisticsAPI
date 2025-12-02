using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Inventory
{
    // 2. DETAYLI LOKASYON (Depocu ürünü bulsun diye)
    public class Inventory : Entity
    {
        public Guid TerminalId { get; private set; }
        public string LocationCode { get; private set; }
        public bool IsVirtual { get; private set; }

        // Adres Detayları
        public string Area { get; private set; }
        public string? Corridor { get; private set; }
        public string? Place { get; private set; }
        public string? Shelf { get; private set; }

        private readonly List<Stock> _stocks = new();
        public IReadOnlyCollection<Stock> Stocks => _stocks.AsReadOnly();

        private Inventory() { }

        public Inventory(Guid terminalId, string area, string? corridor, string? place, string? shelf, bool isVirtual)
        {
            Id = Guid.NewGuid();
            TerminalId = terminalId;
            Area = area;
            Corridor = corridor;
            Place = place;
            Shelf = shelf;
            IsVirtual = isVirtual;
            LocationCode = $"{Area}-{Corridor}-{Place}-{Shelf}".Trim('-');
        }

        // Stok Ekleme Metodu (Domain Logic)
        public InventoryTransaction AddStock(Guid packageId, int quantity, Guid ownerId, InventoryState state, Guid workerId, string? note = null)
        {
            var stock = _stocks.FirstOrDefault(s => s.PackageId == packageId && s.OwnerId == ownerId && s.State == state);

            if (stock != null)
            {
                stock.Increase(quantity);
            }
            else
            {
                stock = new Stock(Id, packageId, quantity, ownerId, state);
                _stocks.Add(stock);
            }

            // Hareket Kaydını Oluştur ve Dön
            return new InventoryTransaction(
                inventoryId: Id,
                packageId: packageId,
                ownerId: ownerId,
                quantityChange: quantity, // Pozitif (+)
                quantityAfter: stock.Quantity, // İşlem sonrası güncel stok (Snapshot)
                type: TransactionType.Inbound,
                workerId: workerId,
                note: note
            );
        }

        // Stok Çıkarma (History Kaydı Dönüyor)
        public InventoryTransaction RemoveStock(Guid packageId, int quantity, Guid ownerId, InventoryState state, Guid workerId, string? note = null)
        {
            var stock = _stocks.FirstOrDefault(s => s.PackageId == packageId && s.OwnerId == ownerId && s.State == state);

            if (stock == null || stock.Quantity < quantity)
            {
                throw new Exception("Yetersiz Stok!");
            }

            stock.Decrease(quantity);

            // Eğer stok 0'a indiyse listeden silebiliriz (veya 0 olarak tutabiliriz, politika meselesi)
            if (stock.Quantity == 0)
            {
                _stocks.Remove(stock);
            }

            return new InventoryTransaction(
                inventoryId: Id,
                packageId: packageId,
                ownerId: ownerId,
                quantityChange: -quantity, // Negatif (-)
                quantityAfter: stock.Quantity, // 0 veya kalan
                type: TransactionType.Outbound,
                workerId: workerId,
                note: note
            );
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