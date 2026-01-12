using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Events.DepartmentEvents;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Companies
{
    public abstract class Company : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string CvrNumber { get; private set; }

        protected Company() { }
        protected Company(string name, string? cvrNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            CvrNumber = cvrNumber;
        }


        #region Worker Factory
        public Worker CreateWorker(Guid appUserId, string fullName, string phone, List<WorkerRole> roles)
        {
            // Şirket kuralları (Örn: Pasif şirkete işçi alınmaz)
            // if (!IsActive) throw new DomainException("Şirket pasif.");

            var worker = new Worker(
                this.Id,    // CompanyId
                this.Id,    // DepartmentId (Varsayılan olarak Merkeze ata veya parametre al)
                appUserId,
                fullName,
                phone,
                roles
            );

            return worker;
        }
        #endregion

        #region Department Factory
        // CreateDepartment: Sadece üretir. Kaydetmek Handler'ın işidir.
        public Department CreateDepartment(string name, Address address, string? contactPhone, string? contactEmail, Guid? managerId)
        {
            var department = new Department(
                this.Id, // CompanyId
                name,
                address,
                contactPhone,
                contactEmail,
                managerId
            );

            // Domain Event fırlatabiliriz (Loglama veya yan etkiler için)
            AddDomainEvent(new DepartmentCreatedEvent(department.Id, this.Id, name));

            return department;
        }

        // Default Department Üretimi (Register sırasında kullanılır)
        public Department CreateDefaultDepartment()
        {
            // Boş bir adres ile varsayılan departman
            var defaultAddress = new Address("", "", "", "", "", new NetTopologySuite.Geometries.Point(0, 0));

            var defaultDept = new Department(
                this.Id,
                "Merkez Ofis", // veya "Headquarters"
                defaultAddress,
                null,
                null,
                null
            );

            // Event fırlat
            AddDomainEvent(new DepartmentCreatedEvent(defaultDept.Id, this.Id, defaultDept.Name));

            return defaultDept;
        }
        #endregion

        // --- UPDATE VE REMOVE METOTLARI NEREYE GİTTİ? ---
        // Cevap: Artık Company içinde değiller.
        // Department bir Aggregate Root olduğu için;
        // UpdateDepartment -> Department.UpdateDetails() (Kendi içinde)
        // RemoveDepartment -> _departmentRepo.Delete() (Handler içinde)
    }
}