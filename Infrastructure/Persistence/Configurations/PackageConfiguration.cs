using Domain.Entities.Inventory;
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

            // 2. DepositPrice (Money) Konfigürasyonu
            // EF Core'a diyoruz ki: "Money ayrı bir tablo değil, bu tablonun kolonlarıdır."
            builder.OwnsOne(p => p.DepositPrice, m =>
            {
                m.Property(x => x.Amount)
                 .HasColumnName("DepositPrice_Amount")
                 .HasColumnType("decimal(18,2)"); // Para için hassasiyet ayarı

                m.Property(x => x.Currency)
                 .HasColumnName("DepositPrice_Currency")
                 .HasMaxLength(3); // "TRY", "USD"
            });

            // 3. Dimensions Konfigürasyonu (Bunu da muhtemelen hata verecektir, şimdiden ekleyelim)
            builder.OwnsOne(p => p.Dimensions, d =>
            {
                d.Property(x => x.Width).HasColumnName("Dim_Width");
                d.Property(x => x.Height).HasColumnName("Dim_Height");
                d.Property(x => x.Depth).HasColumnName("Dim_Depth");
            });
        }
    }
}
