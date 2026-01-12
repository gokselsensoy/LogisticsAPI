using Domain.Entities.Subscriptions;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class AppUserSubscriptionRepository : BaseRepository<AppUserSubscription>, IAppUserSubscription
{
    public AppUserSubscriptionRepository(ApplicationDbContext context) : base(context)
    {
        
    }

    public async Task<AppUserSubscription?> GetAppUserSubscriptionWithAppUserIdAsync(Guid appUserId)
    {
        return await _context.AppUserSubscriptions
                             .FirstOrDefaultAsync(s => s.AppUserId == appUserId);
    }
}
