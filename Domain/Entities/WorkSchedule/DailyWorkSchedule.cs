using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.WorkSchedule
{
    public class DailyWorkSchedule : Entity, IAggregateRoot
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

        public static DailyWorkSchedule CreateFromPattern(WeeklyShiftPattern pattern, DateTime targetDateTime)
        {
            // targetDateTime'dan sadece tarihi alıp DateOnly yapıyoruz
            var targetDateOnly = DateOnly.FromDateTime(targetDateTime);

            // Tarih ile Şablon Saatini birleştirip DateTime oluşturuyoruz
            // pattern.ShiftStart bir TimeSpan'dır (Örn: 09:00)

            var start = targetDateOnly.ToDateTime(TimeOnly.FromTimeSpan(pattern.ShiftStart));

            // Bitiş saati hesaplama (Gece vardiyası kontrolü)
            var end = targetDateOnly.ToDateTime(TimeOnly.FromTimeSpan(pattern.ShiftEnd));

            // Eğer Bitiş saati Başlangıçtan küçükse (Örn: 22:00 - 06:00),
            // Bitiş saati ERTESİ GÜN demektir.
            if (end < start)
            {
                end = end.AddDays(1);
            }

            var schedule = new DailyWorkSchedule
            {
                Id = Guid.NewGuid(),
                WorkerId = pattern.WorkerId,
                Date = targetDateOnly, // DateOnly ataması
                ShiftStart = start,
                ShiftEnd = end
            };

            // --- Detaylar ve Fallback Mantığı (Aynı kalıyor) ---
            if (pattern.Items != null && pattern.Items.Any())
            {
                foreach (var item in pattern.Items)
                {
                    // Item saatlerini de aynı mantıkla DateTime'a çevir
                    var itemStart = targetDateOnly.ToDateTime(TimeOnly.FromTimeSpan(item.StartTime));
                    var itemEnd = targetDateOnly.ToDateTime(TimeOnly.FromTimeSpan(item.EndTime));

                    // Gece vardiyası içindeki item kontrolü
                    // (Basit mantık: Eğer item saati vardiya başından küçükse ertesi gündür)
                    if (itemStart < start) itemStart = itemStart.AddDays(1);
                    if (itemEnd < start) itemEnd = itemEnd.AddDays(1);
                    // Not: Bu hesaplama karmaşık vardiyalarda daha detaylı kontrol gerektirebilir.

                    schedule.AddAllocation(
                        new TimeRange(itemStart, itemEnd),
                        item.Type,
                        item.DefaultVehicleId
                    );
                }
            }
            else if (pattern.DefaultVehicleId.HasValue)
            {
                schedule.AddAllocation(
                    new TimeRange(start, end),
                    AssignmentType.Driving,
                    pattern.DefaultVehicleId
                );
            }

            return schedule;
        }

        // Manuel oluşturma (Şablon dışı mesai)
        public DailyWorkSchedule(Guid workerId, DateOnly date, DateTime start, DateTime end)
        {
            Id = Guid.NewGuid();
            WorkerId = workerId;
            Date = date;
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

        // Vardiya saatlerini güncelleme (Manuel Müdahale)
        public void UpdateShiftTimes(DateTime newStart, DateTime newEnd)
        {
            // Eğer mevcut atamalar yeni saatlerin dışındaysa hata ver
            if (_allocations.Any(a => a.TimeRange.Start < newStart || a.TimeRange.End > newEnd))
                throw new DomainException("Vardiya saatleri, mevcut görevleri dışarıda bırakacak şekilde küçültülemez.");

            ShiftStart = newStart;
            ShiftEnd = newEnd;
        }
    }
}