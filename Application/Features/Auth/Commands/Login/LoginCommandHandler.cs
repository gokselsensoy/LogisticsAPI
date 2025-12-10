using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Auth.DTOs;
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

        public LoginCommandHandler(
            IIdentityService identityService,
            IUserRepository userRepo,
            IWorkerRepository workerRepo,
            ICompanyRepository companyRepo,
            IFreelancerRepository freelancerRepo,
            IIndividualCustomerRepository individualCustomerRepo,
            ICorporateCustomerRepository corporateCustomerRepo)
        {
            _identityService = identityService;
            _userRepo = userRepo;
            _workerRepo = workerRepo;
            _companyRepo = companyRepo;
            _freelancerRepo = freelancerRepo;
            _individualCustomerRepo = individualCustomerRepo;
            _corporateCustomerRepo = corporateCustomerRepo;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken token)
        {
            // 1. IdentityAPI'den Token Al
            // Not: request.ClientType varsa kullanabilirsin, yoksa "multillo_web" hardcoded olabilir.
            // Ben senin kodundaki gibi request.ClientType'ı bıraktım.
            var tokenResponse = await _identityService.LoginAsync(request.Email, request.Password, request.ClientType, token);

            if (tokenResponse == null)
                throw new Exception("Giriş başarısız. Kullanıcı adı veya şifre hatalı.");

            // 2. Yerel Kullanıcıyı (AppUser) Bul
            var localUser = await _userRepo.GetByEmailAsync(request.Email, token);

            if (localUser == null)
                throw new Exception("Kullanıcı sistemde bulunamadı (Senkronizasyon hatası).");

            // 3. TÜM PROFİLLERİ TARA (AvailableProfiles listesini doldur)
            // Artık 'else' yok! Hepsini bağımsız kontrol ediyoruz.
            var profiles = new List<UserProfileDto>();

            // A. WORKER KONTROLÜ
            var worker = await _workerRepo.GetByAppUserIdWithCompanyAsync(localUser.Id, token);
            if (worker != null)
            {
                var company = await _companyRepo.GetByIdAsync(worker.CompanyId, token);

                profiles.Add(new UserProfileDto
                {
                    ProfileType = "Worker",
                    Id = worker.Id,
                    CompanyId = worker.CompanyId,
                    Name = company?.Name ?? "Bilinmeyen Şirket",
                    // Worker'ın içindeki Enum listesini String listesine çeviriyoruz
                    Roles = worker.Roles.Select(r => r.ToString()).ToList()
                });
            }

            // B. FREELANCER KONTROLÜ
            var freelancer = await _freelancerRepo.GetByAppUserIdAsync(localUser.Id, token);
            if (freelancer != null)
            {
                profiles.Add(new UserProfileDto
                {
                    ProfileType = "Freelancer",
                    Id = freelancer.Id,
                    CompanyId = null,
                    Name = freelancer.Name,
                    Roles = new List<string> { "Freelancer" }
                });
            }

            // C. INDIVIDUAL CUSTOMER KONTROLÜ
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

            // D. CORPORATE RESPONSIBLE KONTROLÜ
            var corpCustomer = await _corporateCustomerRepo.GetByResponsibleAppUserIdAsync(localUser.Id, token);
            if (corpCustomer != null)
            {
                // İlgili sorumluyu (Responsible) bul
                var responsible = corpCustomer.Responsibles.First(r => r.AppUserId == localUser.Id);

                profiles.Add(new UserProfileDto
                {
                    ProfileType = "CorporateResponsible",
                    Id = responsible.Id,
                    CompanyId = corpCustomer.Id,
                    Name = corpCustomer.Name,
                    Roles = new List<string> { responsible.Role.ToString() }
                });
            }

            // 4. AKTİF PROFİLİ SEÇ (Default Context Logic)
            if (!profiles.Any())
                throw new Exception("Kullanıcının aktif bir rolü/profili bulunamadı.");

            // Öncelik Sıralaması: Worker > Freelancer > Corporate > Individual
            var activeProfile = profiles.FirstOrDefault(p => p.ProfileType == "Worker")
                                ?? profiles.FirstOrDefault(p => p.ProfileType == "Freelancer")
                                ?? profiles.FirstOrDefault(p => p.ProfileType == "CorporateResponsible")
                                ?? profiles.First();



            // 5. CEVABI DÖN
            return new LoginResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,

                // Seçilen aktif profili UserDto formatına çevir
                CurrentContext = new UserDto
                {
                    Id = localUser.Id,
                    IdentityId = localUser.IdentityId,
                    Email = localUser.Email,
                    FullName = localUser.FullName,

                    // Profil verilerini doldur
                    WorkerId = activeProfile.ProfileType == "Worker" || activeProfile.ProfileType == "Freelancer" ? activeProfile.Id : null,
                    CompanyId = activeProfile.CompanyId,
                    CompanyName = activeProfile.Name,
                    Roles = activeProfile.Roles // Liste olarak atıyoruz
                },

                // Frontend'de "Hesap Değiştir" menüsü için tüm listeyi dönüyoruz
                AvailableProfiles = profiles
            };
        }
    }
}
