using Domain.Entities.Orders;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class OrderGroupRepository : BaseRepository<OrderGroup>, IOrderGroupRepository
    {
        public OrderGroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<OrderGroup?> GetByIdWithOrdersAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<OrderGroup>()
                .Include(og => og.Orders)
                .ThenInclude(o => o.Items)
                .FirstOrDefaultAsync(og => og.Id == id, token);
        }
    }
}
