using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Auth.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Commands.SelectProfile
{
    public class SelectProfileCommandHandler : IRequestHandler<SelectProfileCommand, LoginResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IWorkerRepository _workerRepo;
        private readonly IFreelancerRepository _freelancerRepo;
        private readonly IIndividualCustomerRepository _individualCustomerRepository;
        private readonly ICorporateCustomerRepository _corporateCustomerRepository;
        private readonly ICorporateResponsibleRepository _corporateResponsibleRepository;
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepo;

        public SelectProfileCommandHandler(
            ICurrentUserService currentUserService,
            IWorkerRepository workerRepo,
            IFreelancerRepository freelancerRepo,
            IIndividualCustomerRepository individualCustomerRepository,
            ICorporateCustomerRepository corporateCustomerRepository,
            ICorporateResponsibleRepository corporateResponsibleRepository,
            IIdentityService identityService,
            IUserRepository userRepo)
        {
            _currentUserService = currentUserService;
            _workerRepo = workerRepo;
            _freelancerRepo = freelancerRepo;
            _individualCustomerRepository = individualCustomerRepository;
            _corporateCustomerRepository = corporateCustomerRepository;
            _corporateResponsibleRepository = corporateResponsibleRepository;
            _identityService = identityService;
            _userRepo = userRepo;
        }

        public async Task<LoginResponse> Handle(SelectProfileCommand request, CancellationToken token)
        {
            // 1. Şu anki Token'dan User ID'yi al (Base Token ile gelmiş olmalı)
            var identityId = _currentUserService.UserId;

            // 2. Bu IdentityId'ye karşılık gelen Yerel Kullanıcıyı (AppUser) bul
            // DİKKAT: Burada GetByIdAsync değil, GetByIdentityIdAsync kullanmalıyız!
            var localUser = await _userRepo.GetByIdentityIdAsync(identityId, token);

            if (localUser == null)
                throw new Exception("Kullanıcı profili bulunamadı (Senkronizasyon Hatası).");

            UserProfileDto selectedProfile = null;

            // 2. Profil Tipi'ne göre Doğrulama (User bu profile sahip mi?)
            switch (request.ProfileType)
            {
                case "Worker":
                    // Sadece ID ile çekmek yetmez, AppUserId kontrolü de şart!
                    var worker = await _workerRepo.GetByIdAsync(request.ProfileId, token);
                    if (worker == null || worker.AppUserId != localUser.Id)
                        throw new UnauthorizedAccessException("Bu profile erişim yetkiniz yok.");

                    selectedProfile = new UserProfileDto
                    {
                        ProfileType = "Worker",
                        Id = worker.Id,
                        CompanyId = worker.CompanyId,
                        Roles = worker.Roles.Select(r => r.ToString()).ToList()
                    };
                    break;

                case "Freelancer":
                    var freelancer = await _freelancerRepo.GetByIdAsync(request.ProfileId, token);
                    if (freelancer == null || freelancer.AppUserId != localUser.Id)
                        throw new UnauthorizedAccessException("Bu profile erişim yetkiniz yok.");

                    selectedProfile = new UserProfileDto
                    {
                        ProfileType = "Freelancer",
                        Id = freelancer.Id,
                        CompanyId = null,
                        Roles = new List<string> { "Freelancer" }
                    };
                    break;

                case "IndividualCustomer":
                    var indCustomer = await _individualCustomerRepository.GetByIdAsync(request.ProfileId, token);
                    if (indCustomer == null || indCustomer.AppUserId != localUser.Id)
                        throw new UnauthorizedAccessException("Bu profile erişim yetkiniz yok.");

                    selectedProfile = new UserProfileDto
                    {
                        ProfileType = "IndividualCustomer",
                        Id = indCustomer.Id,
                        CompanyId = null,
                        Roles = new List<string> { "IndividualCustomer" }
                    };
                    break;

                case "CorporateResponsible":
                    // Sadece ID ile çekmek yetmez, AppUserId kontrolü de şart!
                    var responsible = await _corporateResponsibleRepository.GetByIdAsync(request.ProfileId, token);
                    if (responsible == null || responsible.AppUserId != localUser.Id)
                        throw new UnauthorizedAccessException("Bu profile erişim yetkiniz yok.");

                    selectedProfile = new UserProfileDto
                    {
                        ProfileType = "Worker",
                        Id = responsible.Id,
                        CompanyId = responsible.CorporateCustomerId,
                        Roles = responsible.Roles.Select(r => r.ToString()).ToList()
                    };
                    break;

                default:
                    throw new Exception("Geçersiz profil tipi.");
            }

            // 3. YENİ TOKEN ÜRET (Exchange)
            // Artık CompanyId ve Rolleri biliyoruz.
            var tokenResponse = await _identityService.CreateTokenForProfileAsync(
                localUser.IdentityId,
                selectedProfile.CompanyId,
                selectedProfile.ProfileType,
                selectedProfile.Roles,
                request.ClientId,
                token
            );

            // 4. Cevap Dön
            return new LoginResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresIn = tokenResponse.ExpiresIn,
                IsContextSelected = true,
                AvailableProfiles = null // Artık listeye gerek yok, zaten seçti.
            };
        }
    }
}
