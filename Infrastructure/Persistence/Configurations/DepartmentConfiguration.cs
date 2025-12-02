using Domain.Entities.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            // Address Mapping (Flattening)
            builder.OwnsOne(d => d.Address, a =>
            {
                a.Property(p => p.Street).HasColumnName("Address_Street");
                a.Property(p => p.BuildingNo).HasColumnName("Address_BuildingNo");
                a.Property(p => p.ZipCode).HasColumnName("Address_ZipCode");
                a.Property(p => p.City).HasColumnName("Address_City");
                a.Property(p => p.Country).HasColumnName("Address_Country");
                a.Property(p => p.FloorNumber).HasColumnName("Address_FloorNumber");
                a.Property(p => p.FloorLabel).HasColumnName("Address_FloorLabel");
                a.Property(p => p.Door).HasColumnName("Address_Door");
                a.Property(p => p.FormattedAddress).HasColumnName("Address_Full");

                // PostGIS Ayarı
                a.Property(p => p.Location)
                 .HasColumnName("Address_Location")
                 .HasColumnType("geometry (point, 4326)");
            });
        }
    }
}
