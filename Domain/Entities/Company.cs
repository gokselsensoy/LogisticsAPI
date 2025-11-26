using Domain.SeedWork;

namespace Domain.Entities
{
    // -------------------------------------------------------------------------
    // ŞİRKET TİPLERİ
    // -------------------------------------------------------------------------

    // [Company]: Tüzel Kişilik (Base Class)
    // Transporter, Supplier ve Kurumsal Müşteri buradan türer.
    public abstract class Company : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string? CvrNumber { get; private set; }

        // --- Ortak Varlıklar ---
        private readonly List<Vehicle> _fleet = new();
        public IReadOnlyCollection<Vehicle> Fleet => _fleet.AsReadOnly();

        private readonly List<Department> _departments = new();
        public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

        private readonly List<Terminal> _terminals = new();
        public IReadOnlyCollection<Terminal> Terminals => _terminals.AsReadOnly();

        private readonly List<Worker> _workers = new();
        public IReadOnlyCollection<Worker> Workers => _workers.AsReadOnly();

        protected Company(string name, string? cvrNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            CvrNumber = cvrNumber;
        }

        // Ortak Metotlar
        public void AddVehicle(Vehicle vehicle) => _fleet.Add(vehicle);
        public void AddDepartment(Department department) => _departments.Add(department);
    }

    public class Transporter : Company
    {

        public Transporter(string name, string? cvrNumber)
            : base(name, cvrNumber)
        {
        }
    }

    public class Supplier : Company
    {
        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public Supplier(string name, string? cvrNumber) : base(name, cvrNumber) { }

        public void AddProduct(Product product) => _products.Add(product);
    }
}