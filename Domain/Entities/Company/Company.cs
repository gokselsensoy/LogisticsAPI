using Domain.Entities.Departments;
using Domain.Events.DepartmentEvents;
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


        #region Department Methods
        public void AddDepartment(string name, Address address, string? contactPhone, string? contactEmail, Guid? managerId)
        {
            var department = new Department(this.Id, name, address, contactPhone, contactEmail, managerId);
            _departments.Add(department);

            AddDomainEvent(new DepartmentCreatedEvent(department.Id, this.Id, name));
        }

        public void UpdateDepartment(Guid departmentId, string name, Address newAddress, string? phone, string? email, Guid? managerId)
        {
            var dept = _departments.FirstOrDefault(d => d.Id == departmentId);
            if (dept == null) throw new Exception("Departman bulunamadı.");

            dept.UpdateDetails(name, phone, email);
            dept.Relocate(newAddress);
            dept.AssignManager(managerId);

            AddDomainEvent(new DepartmentUpdatedEvent(dept.Id, this.Id));
        }

        public void RemoveDepartment(Guid departmentId)
        {
            var dept = _departments.FirstOrDefault(d => d.Id == departmentId);
            if (dept == null) throw new Exception("Departman bulunamadı.");

            // EF Core bunu "Deleted" olarak işaretler.
            // Bizim Interceptor bunu yakalar, "Deleted"ı iptal eder ve "IsDeleted = true" yapar.
            _departments.Remove(dept);

            AddDomainEvent(new DepartmentDeletedEvent(dept.Id, this.Id));
        }
        #endregion

        public void AddVehicle(Vehicle vehicle) => _fleet.Add(vehicle);

        public void AddTerminal(Terminal terminal) => _terminals.Add(terminal);

        public void AddWorker(Worker worker)
        {
            _workers.Add(worker);
        }

        public Department CreateDefaultDepartment()
        {
            var defaultDept = new Department(this.Id, "Default Department",
                new Address("", "", "", "", "", new Point(0, 0)), "", "", null);

            _departments.Add(defaultDept);
            return defaultDept;
        }
    }
}