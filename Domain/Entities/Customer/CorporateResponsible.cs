using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Customer
{
    public class CorporateResponsible : Entity
    {
        public Guid CorporateId { get; private set; }
        public Guid AppUserId { get; private set; }
        public CorporateRole Role { get; private set; }
        private readonly List<CorporateAddressResponsibleMap> _assignedBranches = new();
        public IReadOnlyCollection<CorporateAddressResponsibleMap> AssignedBranches => _assignedBranches.AsReadOnly();

        public CorporateResponsible(Guid corporateCustomerId, Guid appUserId, CorporateRole role)
        {
            CorporateId = corporateCustomerId;
            AppUserId = appUserId;
            Role = role;
        }

        // Çalışana şube atama metodu
        public void AssignBranch(Guid branchId)
        {
            if (!_assignedBranches.Any(x => x.BranchId == branchId))
            {
                _assignedBranches.Add(new CorporateAddressResponsibleMap(Id, branchId));
            }
        }
    }
}