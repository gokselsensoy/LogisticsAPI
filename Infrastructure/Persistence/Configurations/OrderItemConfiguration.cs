using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id);

            // --- İNDEKSLER ---
            // "Bu üründen toplam kaç tane sattık?" raporu için hızlı arama
            builder.HasIndex(oi => oi.PackageId);


            // --- PROPERTİES ---

            builder.Property(oi => oi.NameSnapshot)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            // Kısmi Teslimat Takibi
            builder.Property(oi => oi.ShippedQuantity)
                   .IsRequired()
                   .HasDefaultValue(0);


            // --- VALUE OBJECTS ---

            // 1. UnitPrice Snapshot (Birim Fiyat)
            builder.OwnsOne(oi => oi.UnitPriceSnapshot, m =>
            {
                m.Property(x => x.Amount).HasColumnName("Price_Amount").HasColumnType("decimal(18,2)");
                m.Property(x => x.Currency).HasColumnName("Price_Currency").HasMaxLength(3);
            });

            // 2. CargoSpec Snapshot (Fiziksel Özellikler)
            builder.OwnsOne(oi => oi.SpecSnapshot, s =>
            {
                s.Property(x => x.Description).HasColumnName("Spec_Desc");
                s.Property(x => x.WeightKg).HasColumnName("Spec_Weight");
                s.Property(x => x.VolumeM3).HasColumnName("Spec_Volume");
            });

            // İlişki zaten OrderConfiguration tarafında tanımlandı ama 
            // OrderItem tarafında da açıkça belirtmek istersen:
            // builder.HasOne<Order>().WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
        }
    }
}
