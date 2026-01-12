using Domain.Entities.Companies;
using Domain.Enums;
using Domain.Events.ProductEvents;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Inventories
{
    public class Product : FullAuditedEntity, IAggregateRoot
    {
        public Guid SupplierId { get; private set; }
        public string Name { get; private set; } // Örn: "Coca Cola Zero"
        public string Description { get; private set; }
        public ProductCategory Category { get; private set; }

        // Bu ürünün dünyadaki en küçük takip birimi nedir?
        // Kola için -> Litre? Hayır, genelde "Şişe" (Piece) veya dökme ise "Litre".
        public UnitType UnitType { get; private set; }

        private readonly List<Package> _packages = new();
        public IReadOnlyCollection<Package> Packages => _packages.AsReadOnly();

        public Supplier Supplier { get; private set; }
        private Product() { }

        public static Product Create(Guid supplierId, string name, string description, UnitType unitType)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                SupplierId = supplierId,
                Name = name,
                Description = description,
                UnitType = unitType
            };

            product.AddDomainEvent(new ProductCreatedEvent(product.Id, supplierId, name));
            return product;
        }

        public void UpdateDetails(string name, string description)
        {
            Name = name;
            Description = description;
            // Event ekleyelim
            AddDomainEvent(new ProductUpdatedEvent(Id, Name));
        }

        // --- PACKAGE MANAGEMENT ---

        public void AddPackage(
            string name,
            PackageType type,
            decimal conversionFactor,
            Money price,
            Dimensions dimensions,
            string barcode,
            bool isReturnable,
            Money? depositPrice)
        {
            // Validasyon: Aynı barkodlu paket eklenemez
            if (_packages.Any(x => x.Barcode == barcode && !x.IsDeleted))
                throw new DomainException("Bu barkoda sahip bir paket zaten var.");

            var package = new Package(Id, name, type, conversionFactor, price, dimensions, barcode, isReturnable, depositPrice);
            _packages.Add(package);

            AddDomainEvent(new PackageAddedToProductEvent(Id, package.Id, name, price.Amount));
        }

        public void UpdatePackage(
        Guid packageId,
        string name,
        Money price,
        Dimensions dimensions,
        bool isReturnable,
        Money? depositPrice)
        {
            var package = _packages.FirstOrDefault(x => x.Id == packageId);
            if (package == null) throw new DomainException("Güncellenecek paket bulunamadı.");

            // Child entity'nin metodunu çağır
            package.UpdateDetails(name, price, dimensions, isReturnable, depositPrice);

            // Audit veya Event
            AddDomainEvent(new PackageUpdatedEvent(Id, packageId, name));
        }

        public void SetCategory(ProductCategory category)
        {
            Category = category;
        }

        public void RemovePackage(Guid packageId)
        {
            var package = _packages.FirstOrDefault(x => x.Id == packageId);
            if (package == null) throw new DomainException("Paket bulunamadı.");

            // Soft Delete (Repository remove çağırmadan entity içinden de yönetilebilir veya IsDeleted setlenir)
            // Ancak FullAuditedEntity kullandığımız için en temizi Repository üzerinden Remove çağırmaktır.
            // Aggregate Root üzerinden listeden çıkarmak EF Core'da delete anlamına gelir (Orphan Removal ayarlıysa).
            // Biz burada sadece listeden çıkarıyoruz, Repository tarafında handle edeceğiz.
            _packages.Remove(package);
        }
    }
}