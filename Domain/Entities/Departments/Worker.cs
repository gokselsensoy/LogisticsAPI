using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Departments
{
    public class Worker : Entity, IAggregateRoot
    {
        public Guid CompanyId { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid AppUserId { get; private set; } // Auth bağlantısı

        private readonly List<WorkerRole> _roles = new();
        public IReadOnlyCollection<WorkerRole> Roles => _roles.AsReadOnly();

        private Worker() { }

        public Worker(Guid companyId, Guid departmentId, Guid appUserId, List<WorkerRole>? roles = null)
        {
            CompanyId = companyId;
            DepartmentId = departmentId;
            AppUserId = appUserId;

            if (roles != null && roles.Any())
            {
                _roles.AddRange(roles);
            }
        }

        public void ChangeDepartment(Guid newDepartmentId)
        {
            DepartmentId = newDepartmentId;
        }

        public void AddRole(WorkerRole role)
        {
            if (!_roles.Contains(role))
            {
                _roles.Add(role);
            }
        }

        public void RemoveRole(WorkerRole role)
        {
            if (_roles.Contains(role))
            {
                _roles.Remove(role);
            }
        }

        public bool HasRole(WorkerRole role) => _roles.Contains(role);
    }
}


// Configure ekle
//public void Configure(EntityTypeBuilder<Worker> builder)
//{
//    // ... Diğer ayarlar ...

//    // Enum Listesini PostgreSQL Array'e çevir
//    builder.Property(e => e.Roles)
//           .HasColumnType("integer[]"); // Veritabanında int array olarak tutar {1, 4, 5}
//}

// kullanım
//var drivers = _context.Workers
//    .Where(w => w.Roles.Contains(WorkerRole.Driver))
//    .ToList();