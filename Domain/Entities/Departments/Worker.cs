using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;

namespace Domain.Entities.Departments
{
    public class Worker : FullAuditedEntity, IAggregateRoot
    {
        public Guid CompanyId { get; private set; }
        public Guid DepartmentId { get; private set; }
        public Guid AppUserId { get; private set; } // Auth bağlantısı
        public string FullName { get; private set; }
        public string Phone { get; private set; }

        private readonly List<WorkerRole> _roles = new();
        public IReadOnlyCollection<WorkerRole> Roles => _roles.AsReadOnly();

        private Worker() { }

        public Worker(Guid companyId, Guid departmentId, Guid appUserId, string fullName, string phone, List<WorkerRole>? roles = null)
        {
            Id = Guid.NewGuid();
            CompanyId = companyId;
            DepartmentId = departmentId;
            AppUserId = appUserId;
            FullName = fullName;
            Phone = phone;

            if (roles != null && roles.Any())
            {
                _roles.AddRange(roles);
            }

            // WorkerCreatedEvent genelde Handler'da fırlatılır (Email şifresi vb. için),
            // ama burada da durabilir. Şimdilik Handler'da fırlatıyorduk, öyle kalsın.
        }

        public void UpdatePersonalDetails(string fullName, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName)) throw new DomainException("İsim boş olamaz.");

            FullName = fullName;
            Phone = phone;
        }

        public void ChangeDepartment(Guid newDepartmentId)
        {
            if (DepartmentId != newDepartmentId)
            {
                DepartmentId = newDepartmentId;
            }
        }

        public void AddRole(WorkerRole role)
        {
            if (!_roles.Contains(role))
            {
                _roles.Add(role);
            }
        }

        public void UpdateRoles(List<WorkerRole> newRoles)
        {
            // Eğer liste null ise temizle
            if (newRoles == null)
            {
                _roles.Clear();
                return;
            }

            var currentRoles = _roles.OrderBy(r => r).ToList();
            var incomingRoles = newRoles.OrderBy(r => r).ToList();

            if (!currentRoles.SequenceEqual(incomingRoles))
            {
                _roles.Clear();
                _roles.AddRange(newRoles);

                // Roller değişti eventi (Güvenlik logları için önemli olabilir)
                // AddDomainEvent(new WorkerRolesUpdatedEvent(Id));
            }
        }

        public void RemoveRole(WorkerRole role)
        {
            if (_roles.Contains(role))
            {
                _roles.Remove(role);
            }
        }

        public void UnlinkUser()
        {
            AppUserId = Guid.Empty;
            // Not: Eğer AppUserId üzerinde veritabanında Foreign Key varsa, 
            // Guid.Empty hataya sebep olabilir. Eğer FK varsa AppUserId'yi nullable (Guid?) yapmalısın.
            // FK yoksa Guid.Empty çalışır.
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