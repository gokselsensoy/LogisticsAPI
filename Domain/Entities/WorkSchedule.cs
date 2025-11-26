using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities
{
    // Örn: "Ahmet - Pazartesi Günleri - 09:00/18:00 Arası - 34ABC53 Aracıyla"
    public class WeeklyShiftPattern : Entity
    {
        public Guid WorkerId { get; private set; }

        public DayOfWeek DayOfWeek { get; private set; } // Monday, Tuesday...

        // Şablon olduğu için DateTime değil TimeSpan kullanıyoruz
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        // Eğer bu gün için varsayılan bir araç varsa (Opsiyonel)
        public Guid? DefaultVehicleId { get; private set; }

        private WeeklyShiftPattern() { }

        public WeeklyShiftPattern(Guid workerId, DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime, Guid? defaultVehicleId = null)
        {
            Id = Guid.NewGuid();
            WorkerId = workerId;
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            DefaultVehicleId = defaultVehicleId;
        }

        public void UpdateTimes(TimeSpan start, TimeSpan end)
        {
            StartTime = start;
            EndTime = end;
        }
    }

    public class DailyWorkSchedule : Entity, IAggregateRoot
    {
        public Guid WorkerId { get; private set; }
        public DateTime Date { get; private set; } // Sadece Tarih (00:00)

        // O günün sınırları (Şablondan gelir ama ezilebilir)
        public DateTime ShiftStart { get; private set; }
        public DateTime ShiftEnd { get; private set; }

        // Detaylı Görev/Araç Atamaları
        private readonly List<ScheduleAllocation> _allocations = new();
        public IReadOnlyCollection<ScheduleAllocation> Allocations => _allocations.AsReadOnly();

        private DailyWorkSchedule() { }

        // Factory Method: Şablondan Günlük Plan Üretme
        // (Bu metodu her Pazar gecesi çalışan bir Job çağıracak)
        public static DailyWorkSchedule CreateFromPattern(WeeklyShiftPattern pattern, DateTime targetDate)
        {
            // Tarih ile Şablon Saatini birleştir
            var start = targetDate.Date.Add(pattern.StartTime);
            var end = targetDate.Date.Add(pattern.EndTime);

            var schedule = new DailyWorkSchedule
            {
                Id = Guid.NewGuid(),
                WorkerId = pattern.WorkerId,
                Date = targetDate.Date,
                ShiftStart = start,
                ShiftEnd = end
            };

            // Şablonda varsayılan araç varsa, otomatik atama yap (Tüm gün için)
            if (pattern.DefaultVehicleId.HasValue)
            {
                // Varsayılan olarak tüm vardiyaya ata
                schedule.AddAllocation(new TimeRange(start, end), AssignmentType.Driving, pattern.DefaultVehicleId);
            }

            return schedule;
        }

        // Manuel oluşturma (Şablon dışı mesai)
        public DailyWorkSchedule(Guid workerId, DateTime date, DateTime start, DateTime end)
        {
            Id = Guid.NewGuid();
            WorkerId = workerId;
            Date = date.Date;
            ShiftStart = start;
            ShiftEnd = end;
        }

        // --- İŞ KURALLARI (Metotlar) ---

        public void AddAllocation(TimeRange range, AssignmentType type, Guid? vehicleId)
        {
            // 1. Kural: Atama, vardiya saatleri dışında olamaz
            if (range.Start < ShiftStart || range.End > ShiftEnd)
                throw new DomainException("Atama, personelin vardiya saatleri dışında olamaz.");

            // 2. Kural: Aynı anda iki iş yapılamaz (Çakışma Kontrolü)
            if (_allocations.Any(a => a.TimeRange.Overlaps(range)))
                throw new DomainException("Bu saat aralığında zaten bir görev atanmış.");

            // 3. Kural: Sürüş görevi ise VehicleId zorunludur
            if (type == AssignmentType.Driving && !vehicleId.HasValue)
                throw new DomainException("Sürüş görevi için araç seçilmelidir.");

            _allocations.Add(new ScheduleAllocation(Id, range, type, vehicleId));
        }

        public void RemoveAllocation(Guid allocationId)
        {
            var item = _allocations.FirstOrDefault(x => x.Id == allocationId);
            if (item != null) _allocations.Remove(item);
        }
    }

    public class ScheduleAllocation : Entity
    {
        public Guid DailyWorkScheduleId { get; private set; }

        public TimeRange TimeRange { get; private set; } // Başlangıç - Bitiş

        public AssignmentType Type { get; private set; } // Ne yapıyor?

        // Eğer araç kullanıyorsa burası dolu olur.
        // Depoda çalışıyorsa veya moladaysa boş (null) olur.
        public Guid? VehicleId { get; private set; }

        private ScheduleAllocation() { }

        public ScheduleAllocation(Guid scheduleId, TimeRange timeRange, AssignmentType type, Guid? vehicleId)
        {
            Id = Guid.NewGuid();
            DailyWorkScheduleId = scheduleId;
            TimeRange = timeRange;
            Type = type;
            VehicleId = vehicleId;
        }
    }
}