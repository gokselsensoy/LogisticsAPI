using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
    {
        public void Configure(EntityTypeBuilder<ShipmentItem> builder)
        {
            builder.ToTable("ShipmentItems");

            // VO: CargoSpec
            builder.OwnsOne(si => si.Spec, s =>
            {
                s.Property(x => x.Description).HasColumnName("Spec_Desc");
                s.Property(x => x.WeightKg).HasColumnName("Spec_Weight");
                s.Property(x => x.VolumeM3).HasColumnName("Spec_Volume");
            });
        }
    }
}
