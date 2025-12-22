using Domain.Entities.WorkSchedule;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IWeeklyShiftPatternRepository : IRepository<WeeklyShiftPattern>
    {
        Task<List<WeeklyShiftPattern>> GetPatternsByDayOfWeekAsync(DayOfWeek day, CancellationToken token);
        Task<WeeklyShiftPattern?> GetByWorkerAndDayAsync(Guid workerId, DayOfWeek dayOfWeek, CancellationToken token);
        Task<WeeklyShiftPattern?> GetByIdWithItemsAsync(Guid id, CancellationToken token);
    }
}
