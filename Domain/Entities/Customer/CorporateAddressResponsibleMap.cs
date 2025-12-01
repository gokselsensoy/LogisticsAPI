using Domain.SeedWork;

namespace Domain.Entities.Customer
{
    public class CorporateAddressResponsibleMap : Entity
    {
        public Guid CorporateWorkerId { get; private set; }
        public Guid BranchId { get; private set; }

        public CorporateAddressResponsibleMap(Guid corporateWorkerId, Guid branchId)
        {
            CorporateWorkerId = corporateWorkerId;
            BranchId = branchId;
        }
    }
}