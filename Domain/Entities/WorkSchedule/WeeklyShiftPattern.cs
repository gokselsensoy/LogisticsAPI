using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;

namespace Domain.Entities.WorkSchedule
{
    public class WeeklyShiftPattern : Entity
    {
        public Guid WorkerId { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeSpan ShiftStart { get; private set; }
        public TimeSpan ShiftEnd { get; private set; }

        // GERİ EKLENDİ: "Tüm gün boyunca kullanılacak varsayılan araç"
        // Eğer detaylı itemlar (ShiftPatternItems) girilmezse bu kullanılır.
        public Guid? DefaultVehicleId { get; private set; }

        // Detaylar (Örn: 10:00-12:00 arası özel görev)
        private readonly List<ShiftPatternItem> _items = new();
        public IReadOnlyCollection<ShiftPatternItem> Items => _items.AsReadOnly();

        private WeeklyShiftPattern() { }

        // Constructor güncellendi
        public WeeklyShiftPattern(
            Guid workerId,
            DayOfWeek dayOfWeek,
            TimeSpan shiftStart,
            TimeSpan shiftEnd,
            Guid? defaultVehicleId = null) // Opsiyonel parametre
        {
            Id = Guid.NewGuid();
            WorkerId = workerId;
            DayOfWeek = dayOfWeek;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            DefaultVehicleId = defaultVehicleId;
        }

        public void AddPatternItem(TimeSpan start, TimeSpan end, AssignmentType type, Guid? vehicleId)
        {
            if (start < ShiftStart || end > ShiftEnd)
                throw new DomainException("Şablon görevi, vardiya saatleri dışında olamaz.");

            _items.Add(new ShiftPatternItem(Id, start, end, type, vehicleId));
        }

        public void UpdateTimes(TimeSpan start, TimeSpan end)
        {
            ShiftStart = start;
            ShiftEnd = end;
        }
    }
}