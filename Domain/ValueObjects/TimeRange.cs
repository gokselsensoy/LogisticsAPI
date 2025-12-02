using Domain.Exceptions;
using Domain.SeedWork;

namespace Domain.ValueObjects
{
    public class TimeRange : ValueObject
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        private TimeRange() { }

        public TimeRange(DateTime start, DateTime end)
        {
            if (end < start) throw new DomainException("Bitiş zamanı başlangıçtan önce olamaz.");
            Start = start;
            End = end;
        }

        public bool Overlaps(TimeRange other)
        {
            return Start < other.End && End > other.Start;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}   
