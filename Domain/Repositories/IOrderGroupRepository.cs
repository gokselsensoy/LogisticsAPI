using Domain.Entities.Orders;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IOrderGroupRepository : IRepository<OrderGroup>
    {
        Task<OrderGroup?> GetByIdWithOrdersAsync(Guid id, CancellationToken token);
    }
}
