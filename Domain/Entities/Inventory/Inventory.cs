using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;

namespace Domain.Entities.Inventory
{
    // 2. DETAYLI LOKASYON (Depocu ürünü bulsun diye)
    public class Inventory : FullAuditedEntity, IAggregateRoot
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
            LocationCode = GenerateLocationCode();
        }

        public static Inventory CreateVirtual(Guid terminalId)
        {
            var inventory = new Inventory();
            inventory.Id = Guid.NewGuid();
            inventory.TerminalId = terminalId;
            inventory.IsVirtual = true;
            inventory.Area = "Virtual";
            inventory.LocationCode = "VIRTUAL"; // Sabit kod

            // Diğer alanlar null kalır (default)
            inventory.Corridor = null;
            inventory.Place = null;
            inventory.Shelf = null;

            return inventory;
        }
        public void Delete()
        {
            if (IsVirtual)
            {
                throw new Exception("Sanal (Virtual) envanter silinemez!");
            }

            // FullAuditedEntity özelliği sayesinde Interceptor bunu yakalayıp Soft Delete yapacak
            IsDeleted = true;
            // Ancak Application katmanında _repo.Remove(this) çağrılmalı ki Interceptor tetiklensin.
            // Veya direkt burada Event fırlatılabilir.
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

        // 1. REZERVASYON İŞLEMİ (Sipariş Gelince)
        // Bu metod fiziksel bir hareket yaratmaz, sadece statü değiştirir.
        public void ReserveStock(Guid packageId, int quantity, Guid ownerId)
        {
            // A. Available (Kullanılabilir) stoğu bul
            var availableStock = _stocks.FirstOrDefault(s => s.PackageId == packageId && s.OwnerId == ownerId && s.State == InventoryState.Available);

            if (availableStock == null || availableStock.Quantity < quantity)
            {
                throw new DomainException("Rezervasyon için yeterli 'Kullanılabilir' (Available) stok yok.");
            }

            // B. Available'dan düş
            availableStock.Decrease(quantity);
            if (availableStock.Quantity == 0) _stocks.Remove(availableStock); // Opsiyonel: 0 ise sil

            // C. Reserved (Rezerve) stoğu bul veya oluştur
            var reservedStock = _stocks.FirstOrDefault(s => s.PackageId == packageId && s.OwnerId == ownerId && s.State == InventoryState.Reserved);

            if (reservedStock != null)
            {
                reservedStock.Increase(quantity);
            }
            else
            {
                // Aynı raf içinde yeni bir Reserved kaydı oluşturuyoruz
                _stocks.Add(new Stock(Id, packageId, quantity, ownerId, InventoryState.Reserved));
            }
        }

        // 2. TOPLAMA / ÇIKIŞ İŞLEMİ (Worker Rafa Gidince)
        // Bu işlem Reserved stoktan düşer ve Transaction (Çıkış) oluşturur.
        public InventoryTransaction PickStock(Guid packageId, int quantity, Guid ownerId, Guid workerId, string? note = null)
        {
            // A. Reserved stoğu bul
            var reservedStock = _stocks.FirstOrDefault(s => s.PackageId == packageId && s.OwnerId == ownerId && s.State == InventoryState.Reserved);

            // Hata Kontrolü: Eğer rezerve edilmemişse, worker bunu toplayamaz!
            if (reservedStock == null || reservedStock.Quantity < quantity)
            {
                throw new DomainException("Toplanmak istenen miktar kadar rezervasyon bulunamadı. Önce rezervasyon yapılmalı.");
            }

            // B. Reserved stoktan düş
            reservedStock.Decrease(quantity);

            if (reservedStock.Quantity == 0)
            {
                _stocks.Remove(reservedStock);
            }

            // C. Transaction Oluştur (Çıkış Hareketi)
            return new InventoryTransaction(
                inventoryId: Id,
                packageId: packageId,
                ownerId: ownerId,
                quantityChange: -quantity, // Azaldı
                quantityAfter: reservedStock.Quantity,
                type: TransactionType.Outbound, // veya 'Picking' diye yeni bir type eklenebilir
                workerId: workerId,
                note: note ?? "Sipariş Toplama"
            );
        }

        // 3. REZERVASYON İPTALİ (Opsiyonel)
        public void CancelReservation(Guid packageId, int quantity, Guid ownerId)
        {
            var reservedStock = _stocks.FirstOrDefault(s => s.PackageId == packageId && s.OwnerId == ownerId && s.State == InventoryState.Reserved);
            if (reservedStock == null || reservedStock.Quantity < quantity) throw new DomainException("İptal edilecek rezervasyon bulunamadı.");

            reservedStock.Decrease(quantity);
            if (reservedStock.Quantity == 0) _stocks.Remove(reservedStock);

            // Tekrar Available'a ekle
            var availableStock = _stocks.FirstOrDefault(s => s.PackageId == packageId && s.OwnerId == ownerId && s.State == InventoryState.Available);
            if (availableStock != null) availableStock.Increase(quantity);
            else _stocks.Add(new Stock(Id, packageId, quantity, ownerId, InventoryState.Available));
        }

        private string GenerateLocationCode()
        {
            // Kod üretme mantığın buraya...
            return $"{Area}-{Corridor}-{Place}-{Shelf}".Trim('-');
        }
    }
}