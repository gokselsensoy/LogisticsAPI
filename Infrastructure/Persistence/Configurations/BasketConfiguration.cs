using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.ToTable("Baskets");

            builder.HasKey(b => b.Id);

            // Bir müşterinin sadece 1 aktif sepeti olabilir mantığı varsa Unique Index atabilirsin.
            // Ama bazen geçmiş sepetleri log olarak tutuyorsan normal index yeterlidir.
            builder.HasIndex(b => b.CustomerId);

            // --- Relationships ---
            builder.HasMany(b => b.Items)
                   .WithOne()
                   .HasForeignKey(bi => bi.BasketId)
                   .OnDelete(DeleteBehavior.Cascade); // Sepet silinirse itemlar da gitsin
        }
    }
}
