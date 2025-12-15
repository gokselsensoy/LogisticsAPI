using Domain.Entities.Customer;
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
        }
    }
}
