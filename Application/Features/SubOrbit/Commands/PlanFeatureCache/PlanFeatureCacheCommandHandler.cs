using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.SubOrbit.Commands.PlanFeatureCache;

public class PlanFeatureCacheCommandHandler : IRequestHandler<PlanFeatureCacheCommand, bool>
{
    private readonly ISubOrbitService _subOrbitService;
    private readonly IPlanFeatureCacheRepository _planFeatureCacheRepository;
    private readonly IUnitOfWork _unitOfWork;
    public PlanFeatureCacheCommandHandler(ISubOrbitService subOrbitService, IPlanFeatureCacheRepository planFeatureCacheRepository,IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _subOrbitService = subOrbitService;
        _planFeatureCacheRepository = planFeatureCacheRepository;
    }
    public async Task<bool> Handle(PlanFeatureCacheCommand request, CancellationToken cancellationToken)
    {
        var response = await _subOrbitService.GetProductFeatureMatrixAsync();
        if (response != null)
        {
            List<Domain.Entities.Subscriptions.PlanFeatureCache> planFeatureCaches = new List<Domain.Entities.Subscriptions.PlanFeatureCache>();
            foreach (var item in response)
            {
                planFeatureCaches.Add(new Domain.Entities.Subscriptions.PlanFeatureCache
                {
                    FeatureId = item.FeatureId,
                    SubOrbitProductId = item.ProductId,
                    FeatureCode = item.FeatureCode,
                    FeatureName = item.FeatureName
                });
            }
            await _planFeatureCacheRepository.AddRangeAsync(planFeatureCaches);
            await _unitOfWork.SaveChangesAsync();
        }

        return true;
    }
}
