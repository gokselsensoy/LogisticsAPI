using Domain.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CorporateCustomerConfiguration : IEntityTypeConfiguration<CorporateCustomer>
    {
        public void Configure(EntityTypeBuilder<CorporateCustomer> builder)
        {
            builder.ToTable("CorporateCustomers");

            builder.HasMany(c => c.Responsibles).WithOne().HasForeignKey(r => r.CorporateCustomerId);
        }
    }
}
