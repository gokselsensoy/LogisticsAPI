using Domain.Entities.Subscriptions;
using Domain.SeedWork;

namespace Domain.Repositories;

public interface IPlanFeatureCacheRepository:IRepository<PlanFeatureCache>
{
    Task<List<PlanFeatureCache>> GetPlanFeatureCacheWithPlanIdAsync(Guid planId);
}
