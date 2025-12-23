using Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            // Enum'ı string olarak tutmak istersen (okunabilirlik için):
            builder.Property(p => p.UnitType)
                   .HasConversion<string>();

            // İlişki Konfigürasyonu (Paketler listesine nasıl erişilecek?)
            builder.Metadata.FindNavigation(nameof(Product.Packages))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);
            // _packages field'ı üzerinden doldurur (Private listeyi doldurmak için)
        }
    }
}
