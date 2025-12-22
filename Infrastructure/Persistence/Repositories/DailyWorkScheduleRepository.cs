using Domain.Entities.WorkSchedule;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class DailyWorkScheduleRepository : BaseRepository<DailyWorkSchedule>, IDailyWorkScheduleRepository
    {
        public DailyWorkScheduleRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<DailyWorkSchedule?> GetByIdWithAllocationsAsync(Guid id, CancellationToken token)
        {
            // Allocation eklerken çakışma kontrolü (Overlap) yapacağımız için 
            // mevcut Allocation'ları belleğe çekmemiz gerekir.
            return await _context.Set<DailyWorkSchedule>()
                .Include(x => x.Allocations)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, token);
        }

        // 1. Zaten Var mı? (Idempotency için)
        public async Task<bool> ExistsAsync(Guid workerId, DateOnly date, CancellationToken token)
        {
            return await _context.Set<DailyWorkSchedule>()
                .AnyAsync(x => x.WorkerId == workerId && x.Date == date && !x.IsDeleted, token);
        }

        // 2. Worker Müsait mi?
        public async Task<bool> IsWorkerAvailableAsync(Guid workerId, DateTime start, DateTime end, CancellationToken token)
        {
            // Adım A: O gün için çalışanın vardiya kaydını bul
            // Not: DateTime'dan DateOnly'e çevirerek arıyoruz
            var targetDate = DateOnly.FromDateTime(start);

            var schedule = await _context.Set<DailyWorkSchedule>()
                .Include(x => x.Allocations) // Detayları (Allocations) çekmemiz şart!
                .Where(x => x.WorkerId == workerId && x.Date == targetDate && !x.IsDeleted)
                .FirstOrDefaultAsync(token);

            // Eğer o gün hiç vardiya kaydı yoksa -> Müsait Değil (Çalışmıyor)
            if (schedule == null) return false;

            // Adım B: İstenen saatler Vardiya Sınırları (ShiftStart - ShiftEnd) içinde mi?
            if (start < schedule.ShiftStart || end > schedule.ShiftEnd)
                return false;

            // Adım C: O saat aralığında başka bir görev (Allocation) var mı?
            // "Overlap" mantığı: (Talep.Basla < Mevcut.Bitis) VE (Talep.Bitis > Mevcut.Basla)
            bool hasConflict = schedule.Allocations
                .Any(a => !a.IsDeleted && (start < a.TimeRange.End && end > a.TimeRange.Start));

            return !hasConflict; // Çakışma yoksa müsaittir.
        }

        // 3. Araç Müsait mi?
        // Bu biraz daha karmaşık çünkü araç herhangi bir çalışanın vardiyasında olabilir.
        public async Task<bool> IsVehicleAvailableAsync(Guid vehicleId, DateTime start, DateTime end, CancellationToken token)
        {
            // Tüm DailyWorkSchedule'ların içindeki Allocation'lara bakacağız.
            // Allocation tablosuna direkt query atabiliriz (Child Entity olsa bile DbSet olarak erişebiliyorsan)
            // Veya Schedule üzerinden gidebiliriz. En performanslısı Allocation üzerinden gitmektir.

            return !await _context.Set<ScheduleAllocation>()
                .AnyAsync(a =>
                    a.VehicleId == vehicleId &&
                    !a.IsDeleted &&
                    (start < a.TimeRange.End && end > a.TimeRange.Start), // Çakışma Kontrolü
                    token);

            // "!" işareti koyduk çünkü AnyAsync "Çakışma var mı?" sorusuna evet derse, müsaitlik "hayır" (false) olur.
        }
    }
}
