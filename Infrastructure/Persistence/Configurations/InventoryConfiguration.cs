using Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("InventoryLocations");
            builder.HasMany(x => x.Stocks).WithOne().HasForeignKey(x => x.InventoryLocationId);

            // LocationCode Unique Olmalı (Aynı depoda aynı raf kodu olamaz)
            builder.HasIndex(x => new { x.TerminalId, x.LocationCode }).IsUnique();
        }
    }
}
