using Domain.Entities.WorkSchedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ShiftPatternItemConfiguration : IEntityTypeConfiguration<ShiftPatternItem>
    {
        public void Configure(EntityTypeBuilder<ShiftPatternItem> builder)
        {
            builder.ToTable("ShiftPatternItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StartTime)
                   .IsRequired()
                   .HasColumnName("StartTime");

            builder.Property(x => x.EndTime)
                   .IsRequired()
                   .HasColumnName("EndTime");

            builder.Property(x => x.Type)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.HasOne<WeeklyShiftPattern>()
                   .WithMany(p => p.Items)
                   .HasForeignKey(x => x.WeeklyShiftPatternId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
