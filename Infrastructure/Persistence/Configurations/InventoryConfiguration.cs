using Domain.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventories");
            builder.HasMany(x => x.Stocks).WithOne().HasForeignKey(x => x.InventoryId);

            // LocationCode Unique Olmalı (Aynı depoda aynı raf kodu olamaz)
            builder.HasIndex(x => new { x.TerminalId, x.LocationCode }).IsUnique();
        }
    }
}
