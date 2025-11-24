using Domain.SeedWork;

namespace Domain.Entities
{
    // -------------------------------------------------------------------------
    // ŞİRKET TİPLERİ
    // -------------------------------------------------------------------------

    // [Company]: Tüzel Kişilik (Base Class)
    // Transporter, Supplier ve Kurumsal Müşteri buradan türer.
    public abstract class Member : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string BillingAddress { get; private set; }

        // Bu şirkette çalışanlar (Worker tablosu ile bağlı)
        private readonly List<Worker> _workers = new();
        public IReadOnlyCollection<Worker> Workers => _workers.AsReadOnly();

        protected Member(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }

    public class TransporterCompany : Member
    {
        public string CvrNumber { get; private set; }
        public TransporterCompany(string name) : base(name) { }
        // Filo, Araçlar vb. buraya eklenecek
    }

    public class SupplierCompany : Member
    {
        public string CvrNumber { get; private set; }
        public SupplierCompany(string name) : base(name) { }
        // Terminaller vb. buraya eklenecek
    }

    // KURUMSAL MÜŞTERİ (Zincir Market/Restoran)
    public class CorporateCustomer : Member
    {
        public string CvrNumber { get; private set; }
        private readonly List<SavedAddress> _addresses = new();
        public IReadOnlyCollection<SavedAddress> Addresses => _addresses.AsReadOnly();

        public CorporateCustomer(string name) : base(name) { }

        public void AddAddress(string name, Location location)
        {
            _branches.Add(new CustomerBranch(Id, name, location));
        }
    }

    public class IndividualCustomer : Member
    {
        public Guid AppUserId { get; private set; }

        // Bireysel müşterinin şubesi olmaz, adres defteri olur.
        private readonly List<SavedAddress> _addresses = new();
        public IReadOnlyCollection<SavedAddress> Addresses => _addresses.AsReadOnly();

        public IndividualCustomer(Guid appUserId)
        {
            Id = Guid.NewGuid();
            AppUserId = appUserId;
        }

        public void AddAddress(string title, Location location)
        {
            _addresses.Add(new SavedAddress(Id, title, location));
        }
    }
}