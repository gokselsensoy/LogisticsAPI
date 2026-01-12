using Domain.Entities.Orders;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IBasketRepository : IRepository<Basket>
    {
        Task<Basket?> GetByIdWithItemsAsync(Guid id, CancellationToken token);
        Task<Basket?> GetByCustomerIdAsync(Guid customerId, CancellationToken token);
    }
}
