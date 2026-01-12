using Domain.Entities.Subscriptions;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PlanFeatureCacheRepository: BaseRepository<PlanFeatureCache>,IPlanFeatureCacheRepository
{
    public PlanFeatureCacheRepository(ApplicationDbContext context) : base(context)
    {
        
    }

    public async Task<List<PlanFeatureCache>> GetPlanFeatureCacheWithPlanIdAsync(Guid planId)
    {
        return await _context.PlanFeatureCache.Where(s => s.SubOrbitProductId == planId).ToListAsync();
    }
}
