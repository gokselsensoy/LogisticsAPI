using Domain.Entities.WorkSchedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class DailyWorkScheduleConfiguration : IEntityTypeConfiguration<DailyWorkSchedule>
    {
        public void Configure(EntityTypeBuilder<DailyWorkSchedule> builder)
        {
            builder.ToTable("DailyWorkSchedules");
            builder.HasMany(x => x.Allocations).WithOne().HasForeignKey(x => x.DailyWorkScheduleId);
        }
    }
}
