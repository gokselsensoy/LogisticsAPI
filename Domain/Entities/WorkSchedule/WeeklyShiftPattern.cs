using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;

namespace Domain.Entities.WorkSchedule
{
    public class WeeklyShiftPattern : FullAuditedEntity, IAggregateRoot
    {
        public Guid WorkerId { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeSpan ShiftStart { get; private set; }
        public TimeSpan ShiftEnd { get; private set; }

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

        #region Add Pattern Item
        public void AddPatternItem(TimeSpan itemStart, TimeSpan itemEnd, AssignmentType type, Guid? vehicleId)
        {
            // 1. Ana Vardiya Sınırlarını Hesapla
            var patternRange = NormalizeTimeRange(ShiftStart, ShiftEnd);

            // 2. Yeni Item Sınırlarını Hesapla
            var newItemRange = NormalizeTimeRange(itemStart, itemEnd);

            // KURAL 1: Vardiya Sınırları Dışına Çıkamaz (Boundary Check)
            if (newItemRange.Start < patternRange.Start || newItemRange.End > patternRange.End)
            {
                throw new DomainException($"Görev ({itemStart}-{itemEnd}), vardiya saatleri ({ShiftStart}-{ShiftEnd}) dışında olamaz.");
            }

            // KURAL 2: Çakışma Kontrolü (Overlap Check) - ARTIK DOĞRU ÇALIŞIR
            foreach (var existingItem in _items)
            {
                // Mevcut item'ı da aynı mantıkla DateTime'a çeviriyoruz
                var existingRange = NormalizeTimeRange(existingItem.StartTime, existingItem.EndTime);

                // DateTime üzerinden çakışma kontrolü (Kesin sonuç verir)
                bool overlaps = newItemRange.Start < existingRange.End && newItemRange.End > existingRange.Start;

                if (overlaps)
                {
                    throw new DomainException($"Bu saat aralığı ({itemStart}-{itemEnd}), mevcut bir görevle çakışıyor.");
                }
            }

            _items.Add(new ShiftPatternItem(Id, itemStart, itemEnd, type, vehicleId));
        }

        private (DateTime Start, DateTime End) NormalizeTimeRange(TimeSpan tStart, TimeSpan tEnd)
        {
            var baseDate = DateTime.Today;

            // 1. Basit Dönüşüm
            var dtStart = baseDate.Add(tStart);
            var dtEnd = baseDate.Add(tEnd);

            // 2. Vardiya Gece Yarısını Geçiyor mu? (Örn: 18:00 - 09:00)
            bool isNightShift = ShiftEnd < ShiftStart;

            // 3. Bitiş saati başlangıçtan küçükse (Örn: 09:00 < 18:00) -> Ertesi gün
            if (tEnd < tStart)
            {
                dtEnd = dtEnd.AddDays(1);
            }
            else if (isNightShift)
            {
                // Vardiya gece yarısını geçiyor AMA bu item geçmiyor (Örn: 01:00 - 03:00)
                // Eğer bu saatler vardiya başlangıcından (18:00) küçükse, bunlar ertesi sabaha aittir.
                if (tStart < ShiftStart)
                {
                    dtStart = dtStart.AddDays(1);
                    dtEnd = dtEnd.AddDays(1);
                }
            }

            return (dtStart, dtEnd);
        }
        #endregion

        public void UpdateShiftDetails(TimeSpan start, TimeSpan end, Guid? defaultVehicleId)
        {
            if (_items.Any(x => x.StartTime < start || x.EndTime > end))
                throw new DomainException("Yeni vardiya saatleri mevcut görevleri dışarıda bırakıyor. Önce görevleri düzenleyin.");

            ShiftStart = start;
            ShiftEnd = end;
            DefaultVehicleId = defaultVehicleId;
        }
    }
}