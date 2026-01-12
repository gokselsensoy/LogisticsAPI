using Domain.Entities.Customers;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IIndividualCustomerRepository : IRepository<IndividualCustomer>
    {
        Task<IndividualCustomer?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token);
    }
}
