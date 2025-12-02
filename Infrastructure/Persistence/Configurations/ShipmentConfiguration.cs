using Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments");

            // Address VO: Pickup
            builder.OwnsOne(s => s.PickupAddress, a =>
            {
                a.Property(p => p.Street).HasColumnName("Pic_Street");
                a.Property(p => p.BuildingNo).HasColumnName("Pic_BuildingNo");
                a.Property(p => p.ZipCode).HasColumnName("Pic_ZipCode");
                a.Property(p => p.City).HasColumnName("Pic_City");
                a.Property(p => p.Country).HasColumnName("Pic_Country");
                a.Property(p => p.FloorNumber).HasColumnName("Pic_FloorNumber");
                a.Property(p => p.FloorLabel).HasColumnName("Pic_FloorLabel");
                a.Property(p => p.Door).HasColumnName("Pic_Door");
                a.Property(p => p.FormattedAddress).HasColumnName("Pic_FormattedAddress");
                a.Property(p => p.Location)
                 .HasColumnName("Pic_Location")
                 .HasColumnType("geometry (point, 4326)");
            });

            // Address VO: Delivery
            builder.OwnsOne(s => s.DeliveryAddress, a =>
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
                 .HasColumnType("geometry (point, 4326)");
            });

            builder.HasMany(s => s.Items).WithOne().HasForeignKey(i => i.ShipmentId);
        }
    }
}
