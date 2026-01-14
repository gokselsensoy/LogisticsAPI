using Domain.Entities.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
    {
        public void Configure(EntityTypeBuilder<Terminal> builder)
        {
            // 1. Tablo Adı (İsteğe bağlı, TPT kullanıyorsan veya standart ad)
            // builder.ToTable("Terminals"); 

            // 2. KRİTİK ÇÖZÜM: Address'in "Owned" (Sahipli) olduğunu belirtiyoruz
            builder.OwnsOne(t => t.Address, a =>
            {
                // Kolon adlarını özelleştiriyoruz ki karışmasın
                a.Property(p => p.Street).HasColumnName("Address_Street");
                a.Property(p => p.BuildingNo).HasColumnName("Address_BuildingNo");
                a.Property(p => p.ZipCode).HasColumnName("Address_ZipCode");
                a.Property(p => p.City).HasColumnName("Address_City");
                a.Property(p => p.Country).HasColumnName("Address_Country");

                // Nullable alanlar
                a.Property(p => p.FloorNumber).HasColumnName("Address_FloorNumber");
                a.Property(p => p.FloorLabel).HasColumnName("Address_FloorLabel");
                a.Property(p => p.Door).HasColumnName("Address_Door");
                a.Property(p => p.FormattedAddress).HasColumnName("Address_FormattedAddress");

                // PostGIS Ayarı (Unutma!)
                a.Property(p => p.Location)
                 .HasColumnName("Address_Location")
                 .HasColumnType("geography (point, 4326)");
            });
        }
    }
}
