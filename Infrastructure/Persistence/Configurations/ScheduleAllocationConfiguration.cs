using Domain.Entities.WorkSchedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ScheduleAllocationConfiguration : IEntityTypeConfiguration<ScheduleAllocation>
    {
        public void Configure(EntityTypeBuilder<ScheduleAllocation> builder)
        {
            builder.ToTable("ScheduleAllocations");

            builder.OwnsOne(sa => sa.TimeRange, tr =>
            {
                tr.Property(t => t.Start).HasColumnName("StartTime");
                tr.Property(t => t.End).HasColumnName("EndTime");
            });
        }
    }
}
