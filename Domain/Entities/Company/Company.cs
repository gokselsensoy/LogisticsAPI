using Domain.Entities.Departments;
using Domain.SeedWork;
using Domain.ValueObjects;
using NetTopologySuite.Geometries;

namespace Domain.Entities.Company
{
    public abstract class Company : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string CvrNumber { get; private set; }

        // --- Ortak Varlıklar ---
        private readonly List<Vehicle> _fleet = new();
        public IReadOnlyCollection<Vehicle> Fleet => _fleet.AsReadOnly();

        private readonly List<Department> _departments = new();
        public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

        private readonly List<Terminal> _terminals = new();
        public IReadOnlyCollection<Terminal> Terminals => _terminals.AsReadOnly();

        private readonly List<Worker> _workers = new();
        public IReadOnlyCollection<Worker> Workers => _workers.AsReadOnly();

        protected Company() { }
        protected Company(string name, string? cvrNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            CvrNumber = cvrNumber;
        }

        // Ortak Metotlar
        public void AddVehicle(Vehicle vehicle) => _fleet.Add(vehicle);

        public void AddDepartment(Department department) => _departments.Add(department);

        public void AddTerminal(Terminal terminal) => _terminals.Add(terminal);

        public void AddWorker(Worker worker)
        {
            _workers.Add(worker);
        }

        public Department CreateDefaultDepartment()
        {
            var defaultDept = new Department("Merkez Ofis", this.Id,
                new Address("", "", "", "", "", new Point(0, 0)));

            _departments.Add(defaultDept);
            return defaultDept;
        }
    }
}