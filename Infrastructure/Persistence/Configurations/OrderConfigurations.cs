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

            // Primary Key
            builder.HasKey(o => o.Id);

            // --- İNDEKSLER (Performans Denormalizasyonu İçin) ---
            // Supplier kendi siparişlerini çekerken çok hızlı olsun
            builder.HasIndex(o => o.SupplierId);
            // Supplier müşteri bazlı arama yaparken hızlı olsun
            builder.HasIndex(o => o.CustomerId);
            // OrderGroup detayları yüklenirken hızlı olsun
            builder.HasIndex(o => o.OrderGroupId);


            // --- PROPERTİES ---

            builder.Property(o => o.ExternalReferenceCode)
                   .HasMaxLength(50);

            builder.Property(o => o.Status)
                   .HasConversion<string>() // Enum'ı string tutmak (Confirmed, Shipped vs.) daha okunaklıdır
                   .HasMaxLength(20);

            builder.Property(o => o.PaymentStatus) // Paid, Refunded
                   .HasConversion<string>()
                   .HasMaxLength(20);

            // İade ve İptal Bilgileri
            builder.Property(o => o.RefundReason).HasMaxLength(250);
            builder.Property(o => o.IsRefunded).HasDefaultValue(false);


            // --- VALUE OBJECTS (OwnsOne) ---

            // 1. Contact (İletişim)
            builder.OwnsOne(o => o.Contact, c =>
            {
                c.Property(p => p.Name).HasColumnName("Contact_Name").HasMaxLength(100);
                c.Property(p => p.Phone).HasColumnName("Contact_Phone").HasMaxLength(20);
                c.Property(p => p.Email).HasColumnName("Contact_Email").HasMaxLength(100);
            });

            // 2. Delivery Address Snapshot (Teslimat Adresi)
            builder.OwnsOne(o => o.DeliveryAddressSnapshot, a =>
            {
                a.Property(p => p.Street).HasColumnName("Del_Street").HasMaxLength(200);
                a.Property(p => p.BuildingNo).HasColumnName("Del_BuildingNo").HasMaxLength(20);
                a.Property(p => p.ZipCode).HasColumnName("Del_ZipCode").HasMaxLength(10);
                a.Property(p => p.City).HasColumnName("Del_City").HasMaxLength(50);
                a.Property(p => p.Country).HasColumnName("Del_Country").HasMaxLength(50);
                a.Property(p => p.FloorNumber).HasColumnName("Del_FloorNumber");
                a.Property(p => p.FloorLabel).HasColumnName("Del_FloorLabel").HasMaxLength(20);
                a.Property(p => p.Door).HasColumnName("Del_Door").HasMaxLength(20);
                a.Property(p => p.FormattedAddress).HasColumnName("Del_FormattedAddress");

                // Coğrafi Konum (PostGIS / SQL Spatial)
                a.Property(p => p.Location)
                 .HasColumnName("Del_Location")
                 .HasColumnType("geography (point, 4326)");
            });

            // 3. TotalPrice (Tedarikçiye Ödenecek Tutar)
            builder.OwnsOne(o => o.TotalPrice, m =>
            {
                m.Property(p => p.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)");
                m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
            });

            // 4. CommissionAmount (Platform Komisyonu) - YENİ
            builder.OwnsOne(o => o.CommissionAmount, m =>
            {
                m.Property(p => p.Amount).HasColumnName("Comm_Amount").HasColumnType("decimal(18,2)");
                m.Property(p => p.Currency).HasColumnName("Comm_Currency").HasMaxLength(3);
            });

            // 5. RefundedAmount (İade Tutarı) - YENİ
            builder.OwnsOne(o => o.RefundedAmount, m =>
            {
                m.Property(p => p.Amount).HasColumnName("Ref_Amount").HasColumnType("decimal(18,2)");
                m.Property(p => p.Currency).HasColumnName("Ref_Currency").HasMaxLength(3);
            });


            // --- İLİŞKİLER (Relationships) ---

            // Order -> OrderGroup (Zorunlu ilişki)
            builder.HasOne<OrderGroup>()
                   .WithMany(og => og.Orders)
                   .HasForeignKey(o => o.OrderGroupId)
                   .OnDelete(DeleteBehavior.Restrict); // OrderGroup silinirse Order'lar silinmesin (veya Cascade olabilir)

            // Order -> OrderItems (Cascade Delete: Order silinirse itemlar çöp olur)
            builder.HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey("OrderId") // Shadow property veya OrderItem içindeki property
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
