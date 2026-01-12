using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.ToTable("ReturnRequests");

            // 1. ContactInfo (Value Object) - Hata veren kısım burasıydı
            builder.OwnsOne(r => r.Contact, c =>
            {
                c.Property(p => p.Name).HasColumnName("Contact_Name").HasMaxLength(100);
                c.Property(p => p.Phone).HasColumnName("Contact_Phone").HasMaxLength(20);
                c.Property(p => p.Email).HasColumnName("Contact_Email").HasMaxLength(100);
            });

            // 2. PickupLocation (Address Value Object)
            // İade nereden alınacak? Bu adres verisini tabloya gömüyoruz.
            builder.OwnsOne(r => r.PickupLocation, a =>
            {
                a.Property(p => p.Street).HasColumnName("Pickup_Street");
                a.Property(p => p.BuildingNo).HasColumnName("Pickup_BuildingNo");
                a.Property(p => p.ZipCode).HasColumnName("Pickup_ZipCode");
                a.Property(p => p.City).HasColumnName("Pickup_City");
                a.Property(p => p.Country).HasColumnName("Pickup_Country");

                a.Property(p => p.FloorNumber).HasColumnName("Pickup_FloorNumber");
                a.Property(p => p.FloorLabel).HasColumnName("Pickup_FloorLabel");
                a.Property(p => p.Door).HasColumnName("Pickup_Door");
                a.Property(p => p.FormattedAddress).HasColumnName("Pickup_Address_Full");

                // PostGIS Ayarı
                a.Property(p => p.Location)
                 .HasColumnName("Pickup_Location")
                 .HasColumnType("geometry (point, 4326)");
            });

            // 3. İlişki: ReturnRequest -> ReturnItems
            builder.HasMany(r => r.Items)
                   .WithOne()
                   .HasForeignKey("ReturnRequestId") // ReturnItem tablosunda oluşacak FK
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
