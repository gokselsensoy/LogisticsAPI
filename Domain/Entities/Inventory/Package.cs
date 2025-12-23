using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Inventory
{
    public class Package : FullAuditedEntity // Fiyatlar vs değişeceği için Audit şart
    {
        public Guid ProductId { get; private set; }

        public string Name { get; private set; } // "6-Pack", "Euro Pallet", "Tekli Şişe"
        public string Barcode { get; private set; } // Barkod her pakette farklıdır (EAN-13, ITF-14)

        public PackageType Type { get; private set; } // Parcel, Pallet...

        public Dimensions Dimensions { get; private set; } // L, W, H, Weight (Desi hesabı için şart)

        // --- MATEMATİK KISMI ---

        // Bu paketin içinde Product'ın BaseUnit'inden kaç tane/kilo/litre var?
        // Örn: 1.5 Litrelik şişe ise ve BaseUnit Litre ise -> 1.5
        // Örn: 1 Koli (24 adet) ise ve BaseUnit Adet ise -> 24
        public decimal ConversionFactor { get; private set; }

        // --- FİYATLANDIRMA ---

        // Bu paketin SATIŞ fiyatı (Birim fiyat * adet DEĞİL. Özel fiyat olabilir)
        public Money Price { get; private set; }

        // Depozito Yönetimi
        public bool IsReturnable { get; private set; } // İade edilebilir mi? (Palet, Cam Şişe)
        public Money? DepositPrice { get; private set; } // Depozito Bedeli (Örn: Palet için 500 TL)

        private Package() { }

        public Package(Guid productId, string name, PackageType type, decimal conversionFactor, Money price, Dimensions dimensions, string barcode, bool isReturnable, Money? depositPrice)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Name = name;
            Type = type;
            ConversionFactor = conversionFactor;
            Price = price;
            Dimensions = dimensions;
            Barcode = barcode;
            IsReturnable = isReturnable;
            DepositPrice = depositPrice;

            if (isReturnable && depositPrice == null)
                throw new DomainException("İade edilebilir paketler için depozito ücreti girilmelidir.");
        }

        public void UpdateDetails(string name, Money price, Dimensions dimensions, bool isReturnable, Money? depositPrice)
        {
            Name = name;
            Price = price;
            Dimensions = dimensions;
            IsReturnable = isReturnable;
            DepositPrice = depositPrice;
        }

        // Birim Fiyatı Hesaplama (Raporlar için computed property)
        public decimal CalculateUnitPrice()
        {
            if (ConversionFactor == 0) return 0;
            return Price.Amount / ConversionFactor;
        }
    }
}