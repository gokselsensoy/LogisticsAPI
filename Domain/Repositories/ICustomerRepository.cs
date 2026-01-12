using Domain.Entities.Customers;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<IndividualCustomer?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token);
        Task<Customer?> GetByIdWithAddressesAsync(Guid id, CancellationToken token);
    }
}
