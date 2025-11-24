using Domain.SeedWork;

namespace Domain.Entities
{
    // =========================================================================
    // ŞUBE (Fiziksel Mekan)
    // =========================================================================

    // =========================================================================
    // KURUMSAL ÇALIŞAN (Yetkili Kişi)
    // =========================================================================
    public class CorporateWorker : Entity
    {
        public Guid CorporateCustomerId { get; private set; }
        public Guid AppUserId { get; private set; } // Login olan kişi
        public CorporateRole Role { get; private set; } // Admin, Bölge Müdürü vb.

        // *** İSTEDİĞİN ÖZELLİK BURADA ***
        // Bu çalışanın sorumlu olduğu şubeler listesi.
        // Eğer bu liste boşsa ve Rolü "Admin" ise -> Tüm şubeleri görür.
        // Eğer listede şube varsa -> Sadece o şubelerin siparişini görür/yönetir.
        private readonly List<WorkerBranchAssignment> _assignedBranches = new();
        public IReadOnlyCollection<WorkerBranchAssignment> AssignedBranches => _assignedBranches.AsReadOnly();

        public CorporateWorker(Guid corporateCustomerId, Guid appUserId, CorporateRole role)
        {
            CorporateCustomerId = corporateCustomerId;
            AppUserId = appUserId;
            Role = role;
        }

        // Çalışana şube atama metodu
        public void AssignBranch(Guid branchId)
        {
            if (!_assignedBranches.Any(x => x.BranchId == branchId))
            {
                _assignedBranches.Add(new WorkerBranchAssignment(Id, branchId));
            }
        }
    }
}