using Domain.Entities.Order;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken token);
    }
}
