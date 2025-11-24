using Domain.SeedWork;

namespace Domain.Entities
{
    // =========================================================================
    // 5. LOGISTICS (FİLO VE ROTA)
    // =========================================================================

    public class Vehicle : Entity
    {
        public Guid CompanyId { get; private set; }
        public Guid DepartmentId { get; private set; }
        public string PlateNumber { get; private set; }
        // ... Kapasite vb ...
    }
}