using Domain.Entities.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RouteTaskConfiguration : IEntityTypeConfiguration<RouteTask>
    {
        public void Configure(EntityTypeBuilder<RouteTask> builder)
        {
            builder.ToTable("RouteTasks");

            // Address VO: Target Location
            builder.OwnsOne(t => t.TargetLocation, a =>
            {
                a.Property(p => p.Street).HasColumnName("Target_Street");
                a.Property(p => p.BuildingNo).HasColumnName("Target_BuildingNo");
                a.Property(p => p.ZipCode).HasColumnName("Target_ZipCode");
                a.Property(p => p.City).HasColumnName("Target_City");
                a.Property(p => p.Country).HasColumnName("Target_Country");
                a.Property(p => p.FloorNumber).HasColumnName("Target_FloorNumber");
                a.Property(p => p.FloorLabel).HasColumnName("Target_FloorLabel");
                a.Property(p => p.Door).HasColumnName("Target_Door");
                a.Property(p => p.FormattedAddress).HasColumnName("Target_FormattedAddress");
                a.Property(p => p.Location)
                 .HasColumnName("Target_Location")
                 .HasColumnType("geometry (point, 4326)");
            });
        }
    }
}
