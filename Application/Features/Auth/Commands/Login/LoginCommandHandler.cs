using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Auth.DTOs;
using Application.Shared.Models;
using Application.Shared.ResultModels;
using Domain.Entities;
using Domain.Entities.Subscriptions;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepo;

    // Tüm Olası Giriş Tipleri İçin Repolar
    private readonly IWorkerRepository _workerRepo;
    private readonly IFreelancerRepository _freelancerRepo;
    private readonly IIndividualCustomerRepository _individualCustomerRepo;
    private readonly ICorporateResponsibleRepository _corporateResponsibleRepo;
    private readonly ICurrentUserService _currentUser;

    private readonly IAppUserSubscription _appUserSubscription;
    private readonly IPlanFeatureCacheRepository _planFeatureCacheRepository;

    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IIdentityService identityService,
        IUserRepository userRepo,
        IWorkerRepository workerRepo,
        IFreelancerRepository freelancerRepo,
        IIndividualCustomerRepository individualCustomerRepo,
        ICorporateResponsibleRepository corporateResponsibleRepo,
        ICurrentUserService currentUser,
        IAppUserSubscription appUserSubscription,
        IPlanFeatureCacheRepository planFeatureCacheRepository,
        ILogger<LoginCommandHandler> logger)
    {
        _identityService = identityService;
        _userRepo = userRepo;
        _workerRepo = workerRepo;
        _freelancerRepo = freelancerRepo;
        _individualCustomerRepo = individualCustomerRepo;
        _corporateResponsibleRepo = corporateResponsibleRepo;
        _currentUser = currentUser;
        _appUserSubscription = appUserSubscription;
        _planFeatureCacheRepository = planFeatureCacheRepository;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken token)
    {
        Guid identityId = _currentUser.UserId;
        string clientId = _currentUser.ClientId;
        _logger.LogInformation(identityId.ToString(), "identityId");
        _logger.LogInformation(clientId, "ClientId");
        if (string.IsNullOrEmpty(clientId))
        {
            // Eğer token içinde client_id yoksa, IdentityServer ayarlarından
            // "IncludeJwtId" veya AccessToken içinde client_id gönder seçeneği açılmalı.
            // Ama genelde default gelir.
            clientId = "multillo_web"; // Fallback (veya hata fırlat)
        }
        AppUser appUser;
        try
        {
            appUser = await _userRepo.GetByIdentityIdAsync(identityId, token);
            _logger.LogInformation(appUser.ToString(), "appUser");
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "appUser ERROR");
            throw;
        }

        // 3. NULL CHECK (Hata alınan yer burasıydı)
        // Eğer Identity'de user var ama senin AppUser tablosunda yoksa bu hatayı fırlatmalıyız.
        if (appUser == null)
        {
            // Burası kritik: Eğer buraya düşüyorsa veri tutarsızlığı vardır 
            // veya kullanıcı register olmadan token almıştır.
            throw new Exception("Kullanıcı sistemde (AppUser) bulunamadı.");
        }

        // Artık appUser'ın null olmadığından eminiz, Id'sini alabiliriz.
        Guid localUserId = appUser.Id;
        // 3. TÜM PROFİLLERİ TOPLA (Worker, Freelancer, Customer...)
        var profiles = new List<UserProfileDto>();

        // A. WORKER
        var workerDataList = await _workerRepo.GetAllByAppUserIdWithCompanyAsync(localUserId, token);
        foreach (var item in workerDataList)
        {
            profiles.Add(new UserProfileDto
            {
                ProfileType = "Worker",
                ProfileId = item.Worker.Id,
                CompanyId = item.Company.Id,
                Name = item.Company.Name,
                Roles = item.Worker.Roles.Select(r => r.ToString()).ToList()
            });
        }

        // B. FREELANCER
        var freelancer = await _freelancerRepo.GetByAppUserIdAsync(localUserId, token);
        if (freelancer != null)
        {
            profiles.Add(new UserProfileDto
            {
                ProfileType = "Freelancer",
                ProfileId = freelancer.Id,
                CompanyId = null,
                Name = freelancer.FullName,
                Roles = new List<string> { "Freelancer" }
            });
        }

        // C. INDIVIDUAL CUSTOMER
        var indCustomer = await _individualCustomerRepo.GetByAppUserIdAsync(localUserId, token);
        if (indCustomer != null)
        {
            profiles.Add(new UserProfileDto
            {
                ProfileType = "IndividualCustomer",
                ProfileId = indCustomer.Id,
                CompanyId = null,
                Name = indCustomer.Name,
                Roles = new List<string> { "IndividualCustomer" }
            });
        }

        // D. CORPORATE RESPONSIBLE
        var corporateDataList = await _corporateResponsibleRepo.GetResponsiblesWithCustomerAsync(localUserId, token);

        foreach (var item in corporateDataList)
        {
            // item.Responsible -> CorporateResponsible entity'si
            // item.Customer    -> CorporateCustomer entity'si (Şirket bilgisi için)

            profiles.Add(new UserProfileDto
            {
                ProfileType = "CorporateResponsible",
                ProfileId = item.Responsible.Id,       // Giriş yapan sorumlu ID'si
                CompanyId = item.Customer.Id,   // Bağlı olduğu Kurumsal Müşteri ID'si
                Name = item.Customer.Name,      // Şirket Adı
                                                // Enum -> String dönüşümü
                Roles = item.Responsible.Roles.Select(r => r.ToString()).ToList()
            });
        }

        // Profil yoksa hata
        if (!profiles.Any())
            throw new Exception("Kullanıcının aktif bir rolü/profili bulunamadı.");


        // 4. KARAR ANI (Single vs Multiple)
        TokenResponse finalTokenResponse;
        bool isContextSelected;

        if (profiles.Count == 1)
        {
            // SENARYO 1: TEK PROFİL (OTOMATİK SEÇİM)
            var p = profiles.First();

            // Base token'ı bırak, "Dolu Token" (CompanyId'li) iste
            finalTokenResponse = await _identityService.CreateTokenForProfileAsync(
                _currentUser.UserId,
                localUserId,
                p.CompanyId,
                p.ProfileType,
                p.ProfileId,
                p.Roles,
                clientId,
                token
            );
            isContextSelected = true;
        }
        else
        {
            finalTokenResponse = null;
            isContextSelected = false;
        }

        // 5. Ödeme sistemi ve yetkilendirme
        var subscription = await _appUserSubscription.GetAppUserSubscriptionWithAppUserIdAsync(appUser.Id);
        var profile = new SubscriptionProfileDto();
        if (subscription is not null)
        {
            profile.PlanName = subscription.PlanName;
            profile.Status = subscription.Status;
            profile.ValidUntil = subscription.ValidUntil;
        }

        var planFeatures = subscription is not null ?
            (await _planFeatureCacheRepository.GetPlanFeatureCacheWithPlanIdAsync(subscription.SubOrbitProductId))
            .Select(f => new PlanFeatureDto
            {
                FeatureCode = f.FeatureCode,
                FeatureName = f.FeatureName
            }).ToList() : new List<PlanFeatureDto>();


        var result = new LoginResponse
        {
            AccessToken = finalTokenResponse?.AccessToken,
            RefreshToken = finalTokenResponse?.RefreshToken,
            ExpiresIn = finalTokenResponse?.ExpiresIn ?? 0,
            AvailableProfiles = profiles,
            IsContextSelected = isContextSelected,
            Features = planFeatures,
            SubscriptionProfile = profile
        };

        return result;
    }
}

