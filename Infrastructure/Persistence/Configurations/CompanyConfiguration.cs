using Domain.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");
            builder.UseTptMappingStrategy();

            builder.HasMany(c => c.Departments).WithOne().HasForeignKey(d => d.CompanyId);
            builder.HasMany(c => c.Workers).WithOne().HasForeignKey(w => w.CompanyId);
        }
    }
}
