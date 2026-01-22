using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
    {
        public void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            builder.ToTable("BasketItems");

            builder.HasKey(bi => bi.Id);

            builder.Property(bi => bi.Quantity).IsRequired();

            // Performans için PackageId (Ürün) ve SupplierId indexlenebilir
            builder.HasIndex(bi => bi.PackageId);
            builder.HasIndex(bi => bi.SupplierId);

            // BasketId zaten FK olarak tanımlı (BasketConfiguration içinde)
        }
    }
}
