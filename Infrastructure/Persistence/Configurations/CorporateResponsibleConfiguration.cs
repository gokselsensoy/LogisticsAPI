using Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CorporateResponsibleConfiguration : IEntityTypeConfiguration<CorporateResponsible>
    {
        public void Configure(EntityTypeBuilder<CorporateResponsible> builder)
        {
            builder.ToTable("CorporateResponsibles");

            builder.Property(w => w.Roles).HasColumnType("integer[]");

            builder.HasOne(cr => cr.CorporateCustomer)
               .WithMany()
               .HasForeignKey(cr => cr.CorporateCustomerId);

            builder.HasMany(cr => cr.AssignedAddresses) // Responsible'dan
               .WithOne(map => map.Responsible)     // Map'teki yeni property'ye
               .HasForeignKey(map => map.ResponsibleId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
