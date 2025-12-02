using Domain.Entities.Company;
using Domain.Entities.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.ToTable("Workers");

            // Roller: PostgreSQL Array (integer[]) olarak tutulur
            builder.Property(w => w.Roles).HasColumnType("integer[]");
        }
    }
}
