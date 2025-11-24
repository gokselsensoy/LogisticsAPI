using Domain.SeedWork;

namespace Domain.Entities
{
    // [Worker]: Hangi AppUser, Hangi Company'de çalışıyor?
    public class Worker : Entity
    {
        public Guid CompanyId { get; private set; }
        public Guid AppUserId { get; private set; }
        public Guid? DepartmentId { get; private set; }
        public WorkerRole Role { get; private set; }

        public Worker(Guid companyId, Guid appUserId, Guid departmentId, WorkerRole role)
        {
            CompanyId = companyId;
            AppUserId = appUserId;
            DepartmentId = departmentId;
            Role = role;
        }
    }
}