using Domain.Entities.Order;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IBasketRepository : IRepository<Basket>
    {
        Task<Basket?> GetByIdWithItemsAsync(Guid id, CancellationToken token);
    }
}
