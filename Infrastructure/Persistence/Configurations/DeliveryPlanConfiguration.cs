using Domain.Entities.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DeliveryPlanConfiguration : IEntityTypeConfiguration<DeliveryPlan>
    {
        public void Configure(EntityTypeBuilder<DeliveryPlan> builder)
        {
            builder.ToTable("DeliveryPlans");

            builder.HasKey(dp => dp.Id);

            builder.Property(dp => dp.TransporterId)
                   .IsRequired();

            builder.Property(dp => dp.PlanDate)
                   .HasColumnType("date")
                   .IsRequired();

            builder.HasMany(dp => dp.Routes)
                   .WithOne()
                   .HasForeignKey("DeliveryPlanId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(DeliveryPlan.Routes))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
