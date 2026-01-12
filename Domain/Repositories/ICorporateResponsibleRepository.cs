using Domain.Entities.Customers;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface ICorporateResponsibleRepository : IRepository<CorporateResponsible>
    {
        Task<CorporateResponsible?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token);
        Task<List<(CorporateResponsible Responsible, CorporateCustomer Customer)>> GetResponsiblesWithCustomerAsync(Guid appUserId, CancellationToken token);
        Task<List<CorporateResponsible>> GetByCorporateIdAsync(Guid corporateId, CancellationToken token);
        Task<CorporateResponsible?> GetByIdWithAssignmentsAsync(Guid id, CancellationToken token);
    }
}
