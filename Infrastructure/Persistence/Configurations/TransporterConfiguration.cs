using Domain.Entities.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TransporterConfiguration : IEntityTypeConfiguration<Transporter>
    {
        public void Configure(EntityTypeBuilder<Transporter> builder)
        {
            builder.ToTable("Transporters"); // Alt Tablo
        }
    }
}
