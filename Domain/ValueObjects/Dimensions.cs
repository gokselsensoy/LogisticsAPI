using Domain.Exceptions;
using Domain.SeedWork;

namespace Domain.ValueObjects
{
    public class Dimensions : ValueObject
    {
        public double Width { get; private set; }  // cm
        public double Height { get; private set; } // cm
        public double Length { get; private set; } // cm
        public double Weight { get; private set; } // kg (Desi hesabı ve lojistik için kritik)

        private Dimensions() { }

        public Dimensions(double width, double height, double length, double weight)
        {
            if (width < 0 || height < 0 || length < 0 || weight < 0)
                throw new DomainException("Boyutlar ve ağırlık negatif olamaz.");

            Width = width;
            Height = height;
            Length = length;
            Weight = weight;
        }

        // Opsiyonel: Desi hesaplama
        public double CalculateVolumetricWeight()
        {
            return (Width * Height * Length) / 3000; // Standart Desi formülü (değişebilir)
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Width;
            yield return Height;
            yield return Length;
            yield return Weight;
        }
    }
}
