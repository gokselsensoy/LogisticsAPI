using Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CorporateAddressResponsibleMapConfiguration : IEntityTypeConfiguration<CorporateAddressResponsibleMap>
    {
        public void Configure(EntityTypeBuilder<CorporateAddressResponsibleMap> builder)
        {
            builder.HasKey(x => new { x.ResponsibleId, x.AddressId });

            // İLİŞKİ TANIMI
            builder.HasOne(m => m.CustomerAddress)
                   .WithMany()
                   .HasForeignKey(m => m.AddressId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
