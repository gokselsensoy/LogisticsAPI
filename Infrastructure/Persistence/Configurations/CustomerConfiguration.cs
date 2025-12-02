using Domain.Entities.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.UseTptMappingStrategy();

            builder.HasMany(c => c.Addresses)
               .WithOne()
               .HasForeignKey("CustomerId")
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
