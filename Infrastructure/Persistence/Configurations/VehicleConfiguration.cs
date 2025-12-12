using Domain.Entities.Company;
using Domain.Entities.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            // PostGIS Konum
            builder.Property(v => v.LastKnownLocation)
                   .HasColumnType("geometry (point, 4326)");

            builder.HasOne<Company>()
               .WithMany() // Department içindeki listeyi de silmiştik (veya sileceksek)
               .HasForeignKey(w => w.CompanyId)
               .IsRequired(false) // Departmansız worker olabilir mi? (Senaryona göre true/false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Department>()
               .WithMany() // Department içindeki listeyi de silmiştik (veya sileceksek)
               .HasForeignKey(w => w.DepartmentId)
               .IsRequired(false) // Departmansız worker olabilir mi? (Senaryona göre true/false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Freelancer>()
               .WithMany() // Department içindeki listeyi de silmiştik (veya sileceksek)
               .HasForeignKey(w => w.FreelancerId)
               .IsRequired(false) // Departmansız worker olabilir mi? (Senaryona göre true/false)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
