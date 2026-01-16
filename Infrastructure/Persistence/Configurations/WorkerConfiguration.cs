using Domain.Entities.Companies;
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

            builder.HasOne<Company>()  // Departman bir Company'ye bağlıdır.
               .WithMany()         // Company'nin birden çok departmanı olabilir (Parametre BOŞ bırakılır çünkü Company'de liste yok).
               .HasForeignKey(d => d.CompanyId) // Bağlantı bu ID ile sağlanır.
               .IsRequired()       // Her departmanın bir şirketi olmak zorundadır.
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.Department)  // Departman bir Company'ye bağlıdır.
                   .WithMany()         // Company'nin birden çok departmanı olabilir (Parametre BOŞ bırakılır çünkü Company'de liste yok).
                   .HasForeignKey(d => d.DepartmentId) // Bağlantı bu ID ile sağlanır.
                   .IsRequired()       // Her departmanın bir şirketi olmak zorundadır.
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
