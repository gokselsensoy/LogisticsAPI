using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.WorkSchedule
{
    public class DailyWorkSchedule : FullAuditedEntity, IAggregateRoot
    {
        public Guid WorkerId { get; private set; }
        public DateOnly Date { get; private set; }

        // DEĞİŞİKLİK 2: DateTime KALDI (Gece vardiyası için gerekli)
        public DateTime ShiftStart { get; private set; }
        public DateTime ShiftEnd { get; private set; }

        // Detaylı Görev/Araç Atamaları
        private readonly List<ScheduleAllocation> _allocations = new();
        public IReadOnlyCollection<ScheduleAllocation> Allocations => _allocations.AsReadOnly();

        private DailyWorkSchedule() { }

        // Manuel oluşturma (Şablon dışı mesai)
        public DailyWorkSchedule(Guid workerId, DateOnly date, DateTime start, DateTime end)
        {
            Id = Guid.NewGuid();
            WorkerId = workerId;
            Date = date;
            ShiftStart = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            ShiftEnd = DateTime.SpecifyKind(end, DateTimeKind.Utc);
        }

        private static DateTime CreateUtcDateTime(DateOnly date, TimeSpan time)
        {
            // Önce Unspecified bir tarih oluşturur
            var dt = date.ToDateTime(TimeOnly.FromTimeSpan(time));
            // Sonra bunu UTC olarak işaretler (Zamanı değiştirmez, sadece etiketi değiştirir)
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }

        public static DailyWorkSchedule CreateFromPattern(WeeklyShiftPattern pattern, DateOnly targetDate)
        {
            // 1. Ana Vardiya Saatlerini DateTime'a çevir
            DateTime shiftStart = CreateUtcDateTime(targetDate, pattern.ShiftStart);
            DateTime shiftEnd = CreateUtcDateTime(targetDate, pattern.ShiftEnd);

            // Gece vardiyası kontrolü (Örn: 22:00 - 06:00)
            if (shiftEnd < shiftStart)
            {
                shiftEnd = shiftEnd.AddDays(1);
            }

            var schedule = new DailyWorkSchedule
            {
                Id = Guid.NewGuid(),
                WorkerId = pattern.WorkerId,
                Date = targetDate,
                ShiftStart = shiftStart,
                ShiftEnd = shiftEnd
            };

            // 2. Pattern Item'larını Allocation'a çevir
            if (pattern.Items != null && pattern.Items.Any())
            {
                foreach (var item in pattern.Items)
                {
                    DateTime itemStart = CreateUtcDateTime(targetDate, item.StartTime);
                    DateTime itemEnd = CreateUtcDateTime(targetDate, item.EndTime);

                    // Item saatleri gece yarısını geçiyorsa düzelt
                    // (Basit mantık: Item başlangıcı vardiya başlangıcından küçükse ertesi gündür)
                    if (itemStart < shiftStart)
                    {
                        itemStart = itemStart.AddDays(1);
                        itemEnd = itemEnd.AddDays(1); // Bitiş de mecburen ertesi gün
                    }
                    else if (itemEnd < itemStart) // Sadece bitiş sarktıysa
                    {
                        itemEnd = itemEnd.AddDays(1);
                    }

                    schedule.AddAllocation(new TimeRange(itemStart, itemEnd), item.Type, item.DefaultVehicleId);
                }
            }
            // 3. Eğer hiç item yoksa ama DefaultVehicle varsa, tüm günü o araca ata (İsteğe bağlı)
            else if (pattern.DefaultVehicleId.HasValue)
            {
                schedule.AddAllocation(new TimeRange(shiftStart, shiftEnd), AssignmentType.Driving, pattern.DefaultVehicleId);
            }

            return schedule;
        }

        public void AddAllocation(TimeRange range, AssignmentType type, Guid? vehicleId)
        {
            // Gelen range içindeki DateTime'ların Kind'ı Unspecified ise UTC yap
            var utcRange = new TimeRange(
                DateTime.SpecifyKind(range.Start, DateTimeKind.Utc),
                DateTime.SpecifyKind(range.End, DateTimeKind.Utc)
            );

            if (utcRange.Start < ShiftStart || utcRange.End > ShiftEnd)
                throw new DomainException($"Atama ({utcRange.Start}-{utcRange.End}), personelin vardiya saatleri ({ShiftStart}-{ShiftEnd}) dışında olamaz.");

            if (_allocations.Any(a => a.TimeRange.Overlaps(utcRange)))
                throw new DomainException("Bu saat aralığında zaten bir görev atanmış.");

            if (type == AssignmentType.Driving && !vehicleId.HasValue)
                throw new DomainException("Sürüş görevi için araç seçilmelidir.");

            _allocations.Add(new ScheduleAllocation(Id, utcRange, type, vehicleId));
        }

        public void RemoveAllocation(Guid allocationId)
        {
            var item = _allocations.FirstOrDefault(x => x.Id == allocationId);
            if (item != null) _allocations.Remove(item);
        }

        public void UpdateShiftTimes(DateTime newStart, DateTime newEnd)
        {
            var utcStart = DateTime.SpecifyKind(newStart, DateTimeKind.Utc);
            var utcEnd = DateTime.SpecifyKind(newEnd, DateTimeKind.Utc);

            if (_allocations.Any(a => a.TimeRange.Start < utcStart || a.TimeRange.End > utcEnd))
                throw new DomainException("Vardiya saatleri, mevcut görevleri dışarıda bırakacak şekilde küçültülemez.");

            ShiftStart = utcStart;
            ShiftEnd = utcEnd;
        }
    }
}