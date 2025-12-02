using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            // VO: Birim Fiyat Snapshot
            builder.OwnsOne(oi => oi.UnitPriceSnapshot, m =>
            {
                m.Property(x => x.Amount).HasColumnName("Price_Amount").HasColumnType("decimal(18,2)");
                m.Property(x => x.Currency).HasColumnName("Price_Currency").HasMaxLength(3);
            });

            // VO: Kargo Özellikleri (CargoSpec)
            builder.OwnsOne(oi => oi.SpecSnapshot, s =>
            {
                s.Property(x => x.Description).HasColumnName("Spec_Desc");
                s.Property(x => x.WeightKg).HasColumnName("Spec_Weight");
                s.Property(x => x.VolumeM3).HasColumnName("Spec_Volume");
            });
        }
    }
}
