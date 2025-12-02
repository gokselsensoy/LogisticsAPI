using Domain.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class FreelancerConfiguration : IEntityTypeConfiguration<Freelancer>
    {
        public void Configure(EntityTypeBuilder<Freelancer> builder)
        {
            builder.ToTable("Freelancers");

            // Araçlar
            builder.HasMany(f => f.Vehicles).WithOne().HasForeignKey(v => v.FreelancerId);
        }
    }
}
