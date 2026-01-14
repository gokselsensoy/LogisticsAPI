using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.OwnsOne(o => o.Contact, c =>
            {
                c.Property(p => p.Name).HasColumnName("Contact_Name").HasMaxLength(100);
                c.Property(p => p.Phone).HasColumnName("Contact_Phone").HasMaxLength(20);
                c.Property(p => p.Email).HasColumnName("Contact_Email").HasMaxLength(100);
            });

            builder.OwnsOne(o => o.DeliveryAddressSnapshot, a =>
            {
                a.Property(p => p.Street).HasColumnName("Del_Street");
                a.Property(p => p.BuildingNo).HasColumnName("Del_BuildingNo");
                a.Property(p => p.ZipCode).HasColumnName("Del_ZipCode");
                a.Property(p => p.City).HasColumnName("Del_City");
                a.Property(p => p.Country).HasColumnName("Del_Country");
                a.Property(p => p.FloorNumber).HasColumnName("Del_FloorNumber");
                a.Property(p => p.FloorLabel).HasColumnName("Del_FloorLabel");
                a.Property(p => p.Door).HasColumnName("Del_Door");
                a.Property(p => p.FormattedAddress).HasColumnName("Del_FormattedAddress");

                a.Property(p => p.Location)
                 .HasColumnName("Del_Location")
                 .HasColumnType("geography (point, 4326)");
            });

            builder.OwnsOne(o => o.PaymentInfo, pi =>
            {
                pi.Property(p => p.Channel).HasColumnName("Payment_Channel");
                pi.Property(p => p.IsProductSettlementRequired).HasColumnName("Payment_ProdSettlement");
                pi.Property(p => p.IsLogisticsSettlementRequired).HasColumnName("Payment_LogSettlement");
            });

            // 4. TotalPrice (Value Object - Para)
            builder.OwnsOne(o => o.TotalPrice, m =>
            {
                m.Property(p => p.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)");
                m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
            });

            // 5. OrderItems (İlişki)
            builder.HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey("OrderId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
