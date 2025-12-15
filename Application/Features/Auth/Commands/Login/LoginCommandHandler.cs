using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Auth.DTOs;
using Application.Shared.Models;
using Domain.Entities.Departments;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepo;

        // Tüm Olası Giriş Tipleri İçin Repolar
        private readonly IWorkerRepository _workerRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly IFreelancerRepository _freelancerRepo;
        private readonly IIndividualCustomerRepository _individualCustomerRepo;
        private readonly ICorporateCustomerRepository _corporateCustomerRepo;
        private readonly ICorporateResponsibleRepository _corporateResponsibleRepo;

        public LoginCommandHandler(
            IIdentityService identityService,
            IUserRepository userRepo,
            IWorkerRepository workerRepo,
            ICompanyRepository companyRepo,
            IFreelancerRepository freelancerRepo,
            IIndividualCustomerRepository individualCustomerRepo,
            ICorporateCustomerRepository corporateCustomerRepo,
            ICorporateResponsibleRepository corporateResponsibleRepo)
        {
            _identityService = identityService;
            _userRepo = userRepo;
            _workerRepo = workerRepo;
            _companyRepo = companyRepo;
            _freelancerRepo = freelancerRepo;
            _individualCustomerRepo = individualCustomerRepo;
            _corporateCustomerRepo = corporateCustomerRepo;
            _corporateResponsibleRepo = corporateResponsibleRepo;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken token)
        {
            // 1. ADIM: LoginAsync ile Doğrulama Yap (ValidateUserAsync yerine)
            // Bu metod token dönerse şifre doğrudur.
            var baseTokenResponse = await _identityService.LoginAsync(request.Email, request.Password, request.ClientType, token);

            if (baseTokenResponse == null)
                throw new Exception("Giriş başarısız. Kullanıcı adı veya şifre hatalı.");

            // 2. Yerel Kullanıcıyı (AppUser) Bul
            var localUser = await _userRepo.GetByEmailAsync(request.Email, token);
            if (localUser == null)
                throw new Exception("Kullanıcı sistemde bulunamadı (Senkronizasyon hatası).");

            // 3. TÜM PROFİLLERİ TOPLA (Worker, Freelancer, Customer...)
            var profiles = new List<UserProfileDto>();

            // A. WORKER
            var workerDataList = await _workerRepo.GetAllByAppUserIdWithCompanyAsync(localUser.Id, token);
            foreach (var item in workerDataList)
            {
                profiles.Add(new UserProfileDto
                {
                    ProfileType = "Worker",
                    Id = item.Worker.Id,
                    CompanyId = item.Company.Id,
                    Name = item.Company.Name,
                    Roles = item.Worker.Roles.Select(r => r.ToString()).ToList()
                });
            }

            // B. FREELANCER
            var freelancer = await _freelancerRepo.GetByAppUserIdAsync(localUser.Id, token);
            if (freelancer != null)
            {
                profiles.Add(new UserProfileDto
                {
                    ProfileType = "Freelancer",
                    Id = freelancer.Id,
                    CompanyId = null,
                    Name = freelancer.FullName,
                    Roles = new List<string> { "Freelancer" }
                });
            }

            // C. INDIVIDUAL CUSTOMER
            var indCustomer = await _individualCustomerRepo.GetByAppUserIdAsync(localUser.Id, token);
            if (indCustomer != null)
            {
                profiles.Add(new UserProfileDto
                {
                    ProfileType = "IndividualCustomer",
                    Id = indCustomer.Id,
                    CompanyId = null,
                    Name = indCustomer.Name,
                    Roles = new List<string> { "IndividualCustomer" }
                });
            }

            // D. CORPORATE RESPONSIBLE
            var corporateDataList = await _corporateResponsibleRepo.GetResponsiblesWithCustomerAsync(localUser.Id, token);

            foreach (var item in corporateDataList)
            {
                // item.Responsible -> CorporateResponsible entity'si
                // item.Customer    -> CorporateCustomer entity'si (Şirket bilgisi için)

                profiles.Add(new UserProfileDto
                {
                    ProfileType = "CorporateResponsible",
                    Id = item.Responsible.Id,       // Giriş yapan sorumlu ID'si
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
                    localUser.IdentityId,
                    p.CompanyId,
                    p.ProfileType,
                    p.Roles,
                    request.ClientType,
                    token
                );
                isContextSelected = true;
            }
            else
            {
                // SENARYO 2: ÇOKLU PROFİL (MANUEL SEÇİM GEREKİYOR)
                // İlk başta aldığımız Base Token'ı kullanıyoruz.
                // Bu token ile henüz şirket işlemleri yapamaz ama SelectProfile endpoint'ine istek atabilir.
                finalTokenResponse = baseTokenResponse;
                isContextSelected = false;
            }

            // 5. CEVAP
            return new LoginResponse
            {
                AccessToken = finalTokenResponse.AccessToken,
                RefreshToken = finalTokenResponse.RefreshToken,
                ExpiresIn = finalTokenResponse.ExpiresIn,
                AvailableProfiles = profiles,
                IsContextSelected = isContextSelected
            };
        }
    }
}
