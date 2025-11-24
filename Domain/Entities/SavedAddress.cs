using Domain.SeedWork;

namespace Domain.Entities
{
    // ŞUBE (Fiziksel Mekan - Sipariş buradan verilir)
    public class SavedAddress : Entity
    {
        public Guid CustomerId { get; private set; }
        public string Title { get; private set; } // "Kadıköy Şubesi"
        public Location Location { get; private set; } // Koordinat
        public AddressType AddressType { get; private set; }

        public SavedAddress(Guid customerId, string title, Location location)
        {
            CustomerId = customerId;
            Title = title;
            Location = location;
        }
    }
}