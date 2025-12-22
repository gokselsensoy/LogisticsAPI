using Application.Features.Schedules.Commands.GenerateSchedule.DTOs;
using Domain.Entities.WorkSchedule;
using Domain.Repositories;
using Domain.SeedWork;

namespace Application.Features.Schedules.Commands.GenerateSchedule.Services
{
    public class ScheduleGeneratorService : IScheduleGeneratorService
    {
        private readonly IWeeklyShiftPatternRepository _patternRepo;
        private readonly IDailyWorkScheduleRepository _dailyScheduleRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleGeneratorService(
            IWeeklyShiftPatternRepository patternRepo,
            IDailyWorkScheduleRepository dailyScheduleRepo,
            IUnitOfWork unitOfWork)
        {
            _patternRepo = patternRepo;
            _dailyScheduleRepo = dailyScheduleRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task GenerateSchedulesAsync(GenerateScheduleRequest request, CancellationToken token)
        {
            // request.StartDate (Genelde yarın)
            // request.EndDate (Genelde 1 hafta veya 1 ay sonrası)

            // 1. İlgili Tarih Aralığında Döngü Kur
            for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
            {
                var targetDateOnly = DateOnly.FromDateTime(date);
                var dayOfWeek = date.DayOfWeek;

                // 2. O gün için şablonu olan TÜM Workerları çek
                // (Performans notu: Bunu döngü dışına alıp toplu çekmek daha iyidir ama anlaşılırlık için burada)
                var activePatterns = await _patternRepo.GetPatternsByDayOfWeekAsync(dayOfWeek, token);

                foreach (var pattern in activePatterns)
                {
                    // 3. IDEMPOTENCY CHECK (Daha önce oluşturulmuş mu?)
                    // Eğer o gün için zaten bir schedule varsa, tekrar oluşturma (manuel düzenlemeleri ezmeyelim)
                    bool exists = await _dailyScheduleRepo.ExistsAsync(pattern.WorkerId, targetDateOnly, token);

                    if (!exists)
                    {
                        // 4. Factory Metodunu Çağır
                        var dailySchedule = DailyWorkSchedule.CreateFromPattern(pattern, targetDateOnly);

                        // 5. Repoya Ekle
                        _dailyScheduleRepo.Add(dailySchedule);
                    }
                }
            }

            // 6. Hepsini Kaydet
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
