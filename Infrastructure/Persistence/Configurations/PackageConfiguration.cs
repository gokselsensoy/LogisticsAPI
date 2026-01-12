using Domain.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            // 1. Tablo Adı (İsteğe bağlı)
            builder.ToTable("Packages");

            builder.Property(p => p.Id)
                   .ValueGeneratedNever();

            builder.Property(p => p.Name)
                               .IsRequired()
                               .HasMaxLength(200);

            builder.Property(p => p.Barcode)
                   .HasMaxLength(50);

            // Barcode Unique (Benzersiz) olmalı (Silinmemişler arasında)
            // Eğer veritabanın destekliyorsa bu index'i atabilirsin.
            // builder.HasIndex(p => p.Barcode).IsUnique().HasFilter("\"IsDeleted\" = false");

            // --- 2. Money: PRICE (Satış Fiyatı - Zorunlu) ---
            builder.OwnsOne(p => p.Price, m =>
            {
                m.Property(x => x.Amount)
                 .HasColumnName("Price_Amount") // Kolon adı çakışmasın diye prefix
                 .HasColumnType("decimal(18,2)")
                 .IsRequired(); // Satış fiyatı zorunlu

                m.Property(x => x.Currency)
                 .HasColumnName("Price_Currency")
                 .HasMaxLength(3)
                 .IsRequired();
            });

            // --- 3. Money: DEPOSIT PRICE (Depozito - Opsiyonel) ---
            builder.OwnsOne(p => p.DepositPrice, m =>
            {
                m.Property(x => x.Amount)
                 .HasColumnName("Deposit_Amount") // Prefix farklı olmalı!
                 .HasColumnType("decimal(18,2)");
                // IsRequired DEMİYORUZ, çünkü DepositPrice nullable (?)

                m.Property(x => x.Currency)
                 .HasColumnName("Deposit_Currency")
                 .HasMaxLength(3);
            });

            // --- 4. Dimensions (Boyutlar ve Ağırlık) ---
            builder.OwnsOne(p => p.Dimensions, d =>
            {
                d.Property(x => x.Width).HasColumnName("Dim_Width");
                d.Property(x => x.Height).HasColumnName("Dim_Height");
                d.Property(x => x.Length).HasColumnName("Dim_Length");

                // UNUTTUĞUN KISIM BURASIYDI:
                d.Property(x => x.Weight).HasColumnName("Dim_Weight");
            });

            // --- 5. İlişkiler ---
            // Package, Product'a bağlıdır.
            // Product silinirse Package de silinsin (Cascade)
            builder.HasOne(p => p.Product)
                   .WithMany(product => product.Packages)
                   .HasForeignKey(package => package.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
