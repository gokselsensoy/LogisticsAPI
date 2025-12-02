using Domain.Entities.WorkSchedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class WeeklyShiftPatternConfiguration : IEntityTypeConfiguration<WeeklyShiftPattern>
    {
        public void Configure(EntityTypeBuilder<WeeklyShiftPattern> builder)
        {
            builder.ToTable("WeeklyShiftPatterns");
            builder.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.WeeklyShiftPatternId);
        }
    }
}
