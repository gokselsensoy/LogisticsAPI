using Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            builder.ToTable("CustomerAddresses");

            builder.OwnsOne(ca => ca.Address, a =>
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
                a.Property(p => p.Location).HasColumnType("geometry (point, 4326)");
            });
        }
    }
}
