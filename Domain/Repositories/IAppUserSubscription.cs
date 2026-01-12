using Domain.Entities.Subscriptions;
using Domain.SeedWork;

namespace Domain.Repositories;

public interface IAppUserSubscription : IRepository<AppUserSubscription>
{
    Task<AppUserSubscription?> GetAppUserSubscriptionWithAppUserIdAsync(Guid appUserId);
}
