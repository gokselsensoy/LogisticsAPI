using Domain.SeedWork;

namespace Domain.ValueObjects
{
    public class CargoSpec : ValueObject
    {
        public string Description { get; private set; } // "Kırmızı Koli"
        public double WeightKg { get; private set; }
        public double VolumeM3 { get; private set; }

        private CargoSpec() { }

        public CargoSpec(string desc, double weight, double volume)
        {
            Description = desc; WeightKg = weight; VolumeM3 = volume;
        }
        protected override IEnumerable<object> GetEqualityComponents() { yield return Description; yield return WeightKg; }
    }
}