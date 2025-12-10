using Domain.SeedWork;

namespace Domain.ValueObjects
{
    // Value Object: Boyutlar
    public class Dimensions : ValueObject
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public double Depth { get; private set; }
        public double VolumeM3 => Width * Height * Depth;
        private Dimensions() { }

        public Dimensions(double width, double height, double depth)
        {
            Width = width; Height = height; Depth = depth;
        }

        protected override IEnumerable<object> GetEqualityComponents() { yield return VolumeM3; }
    }
}
