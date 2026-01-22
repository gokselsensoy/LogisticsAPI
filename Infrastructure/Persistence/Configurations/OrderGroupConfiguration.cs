using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderGroupConfiguration : IEntityTypeConfiguration<OrderGroup>
    {
        public void Configure(EntityTypeBuilder<OrderGroup> builder)
        {
            builder.ToTable("OrderGroups");

            // Primary Key (Base entity'den gelir ama açıkça belirtmek iyidir)
            builder.HasKey(og => og.Id);

            // --- Properties ---

            // Sipariş Numarası benzersiz olmalı ve indexlenmeli
            builder.Property(og => og.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnName("OrderNumber");

            builder.HasIndex(og => og.OrderNumber).IsUnique();

            // Müşteri ID indexlenmeli (Siparişlerim sayfası için)
            builder.Property(og => og.CustomerId).IsRequired();
            builder.HasIndex(og => og.CustomerId);

            builder.Property(og => og.PaymentStatus)
                   .HasConversion<string>() // Enum'ı string olarak tutmak okunabilirlik sağlar (Paid, Pending vs.)
                   .HasMaxLength(20);

            // --- Value Objects ---

            // 1. PaymentInfo (PaymentContext)
            builder.OwnsOne(og => og.PaymentInfo, pi =>
            {
                pi.Property(p => p.Channel)
                  .HasColumnName("Payment_Channel")
                  .HasConversion<string>() // Veya int kalsın istersen kaldırırsın
                  .HasMaxLength(50);

                pi.Property(p => p.ExternalTransactionId)
                  .HasColumnName("Payment_TransactionId")
                  .HasMaxLength(100); // Iyzico/Stripe ID'leri uzun olabilir

                pi.Property(p => p.IsProductSettlementRequired)
                  .HasColumnName("Payment_ProdSettlement");

                pi.Property(p => p.IsLogisticsSettlementRequired)
                  .HasColumnName("Payment_LogSettlement");
            });

            // 2. TotalPrice (Money)
            builder.OwnsOne(og => og.TotalPrice, m =>
            {
                m.Property(p => p.Amount)
                 .HasColumnName("TotalAmount")
                 .HasColumnType("decimal(18,2)"); // Para için standart

                m.Property(p => p.Currency)
                 .HasColumnName("Currency")
                 .HasMaxLength(3);
            });

            // --- Relationships ---

            // OrderGroup silinirse altındaki Order'lar da silinsin mi? 
            // Genelde sipariş verisi fiziksel silinmez (Soft Delete) ama ilişki tanımı budur:
            builder.HasMany(og => og.Orders)
                   .WithOne()
                   .HasForeignKey(o => o.OrderGroupId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
