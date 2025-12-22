using Domain.Entities.WorkSchedule;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WeeklyShiftPatternRepository : BaseRepository<WeeklyShiftPattern>, IWeeklyShiftPatternRepository
    {
        public WeeklyShiftPatternRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<WeeklyShiftPattern>> GetPatternsByDayOfWeekAsync(DayOfWeek day, CancellationToken token)
        {
            return await _context.WeeklyShiftPatterns
                .Include(p => p.Items) // Items önemli!
                .Where(p => p.DayOfWeek == day && !p.IsDeleted && p.IsActive)
                .ToListAsync(token);
        }

        public async Task<WeeklyShiftPattern?> GetByWorkerAndDayAsync(Guid workerId, DayOfWeek dayOfWeek, CancellationToken token)
        {
            // Silinmemiş ve o çalışana ait, o günkü şablonu getir
            return await _context.Set<WeeklyShiftPattern>()
                .Include(x => x.Items) // Item'ları da getirelim ki dolu mu boş mu görelim
                .FirstOrDefaultAsync(x => x.WorkerId == workerId && x.DayOfWeek == dayOfWeek && !x.IsDeleted, token);
        }

        public async Task<WeeklyShiftPattern?> GetByIdWithItemsAsync(Guid id, CancellationToken token)
        {
            // Update işlemi yaparken alt detayları (Items) kontrol edeceğimiz için Include şart
            return await _context.Set<WeeklyShiftPattern>()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, token);
        }
    }
}
