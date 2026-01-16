using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Auth.DTOs;
using Application.Shared.ResultModels;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Queries.GetMyProfile
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, MyProfileDto>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IWorkerRepository _workerRepo;
        private readonly IIndividualCustomerRepository _individualRepo;
        private readonly IFreelancerRepository _freelancerRepo;
        private readonly ICorporateResponsibleRepository _responsibleRepo;
        // UserRepo'ya temel bilgiler (Email vs) için ihtiyaç olabilir
        private readonly IUserRepository _userRepo;

        public GetMyProfileQueryHandler(
            ICurrentUserService currentUser,
            IWorkerRepository workerRepo,
            IIndividualCustomerRepository individualRepo,
            IFreelancerRepository freelancerRepo,
            ICorporateResponsibleRepository responsibleRepo,
            IUserRepository userRepo)
        {
            _currentUser = currentUser;
            _workerRepo = workerRepo;
            _individualRepo = individualRepo;
            _freelancerRepo = freelancerRepo;
            _responsibleRepo = responsibleRepo;
            _userRepo = userRepo;
        }

        public async Task<MyProfileDto> Handle(GetMyProfileQuery request, CancellationToken token)
        {
            // 1. Token Validasyon
            if (!_currentUser.ProfileId.HasValue || string.IsNullOrEmpty(_currentUser.ProfileType))
            {
                throw new UnauthorizedAccessException("Geçerli bir profil seçimi yapılmamış.");
            }

            Guid profileId = _currentUser.ProfileId.Value;
            string profileType = _currentUser.ProfileType;
            Guid appUserId = _currentUser.AppUserId;

            // Temel kullanıcı bilgilerini (Email, Phone) AppUser tablosundan alalım
            // (Eğer bu bilgiler alt tablolarda da varsa direkt oradan da alabilirsin)
            var appUser = await _userRepo.GetByIdAsync(appUserId, token);

            var response = new MyProfileDto
            {
                AppUserId = appUserId,
                ProfileId = profileId,
                ProfileType = profileType,
                Email = appUser?.Email,
                Phone = null
            };

            // 2. TİPE GÖRE AYRIŞTIRMA (Switch Case)
            switch (profileType)
            {
                case "Worker":
                    await LoadWorkerProfile(response, profileId, token);
                    break;

                case "IndividualCustomer":
                    await LoadIndividualCustomerProfile(response, profileId, token);
                    break;

                case "Freelancer":
                    await LoadFreelancerProfile(response, profileId, token);
                    break;

                case "CorporateResponsible":
                    await LoadCorporateResponsibleProfile(response, profileId, token);
                    break;

                default:
                    throw new Exception($"Bilinmeyen profil tipi: {profileType}");
            }

            return response;
        }

        // --- YARDIMCI METOTLAR (Kod Okunabilirliği İçin) ---

        private async Task LoadWorkerProfile(MyProfileDto response, Guid workerId, CancellationToken token)
        {
            // Repo'da Include(w => w.Company) ve Include(w => w.Roles) yapılmalı
            var worker = await _workerRepo.GetByIdWithCompanyAsync(workerId, token);
            if (worker == null) throw new Exception("Worker profili bulunamadı.");

            response.FullName = worker.FullName ?? "Worker User"; // Worker tablosunda name varsa
            response.Roles = worker.Roles.Select(r => r.ToString()).ToList();
            response.Phone = worker.Phone;

            // Şirket Bilgisi
            response.Organization = new OrganizationInfoDto
            {
                Id = worker.Department.CompanyId, // Relation üzerinden
                Name = worker.Department.Company.Name,
                Type = "CompanyWorker", // Veya TPT'den gelen gerçek tip (Transporter/Supplier)
                IsAdmin = worker.Roles.Any(r => r == WorkerRole.Admin) // Örnek kontrol
            };
        }

        private async Task LoadCorporateResponsibleProfile(MyProfileDto response, Guid responsibleId, CancellationToken token)
        {
            // Repo'da Include(r => r.CorporateCustomer) yapılmalı
            var responsible = await _responsibleRepo.GetByIdWithCustomerAsync(responsibleId, token);
            if (responsible == null) throw new Exception("Kurumsal Sorumlu profili bulunamadı.");

            response.FullName = responsible.FullName;
            response.Roles = responsible.Roles.Select(r => r.ToString()).ToList();
            response.Phone = responsible.Phone;

            // Kurumsal Müşteri Bilgisi
            response.Organization = new OrganizationInfoDto
            {
                Id = responsible.CorporateCustomerId,
                Name = responsible.CorporateCustomer?.Name ?? "Şirket", // Nav Property
                Type = "CorporateCustomer",
                IsAdmin = responsible.Roles.Any(r => r == CorporateRole.Admin) // Örnek kontrol
            };
        }

        private async Task LoadIndividualCustomerProfile(MyProfileDto response, Guid customerId, CancellationToken token)
        {
            var customer = await _individualRepo.GetByIdAsync(customerId, token);
            if (customer == null) throw new Exception("Bireysel Müşteri profili bulunamadı.");

            response.FullName = customer.Name; // IndividualCustomer tablosundaki ad
            response.Roles = new List<string> { "IndividualCustomer" };
            response.Phone = customer.PhoneNumber;
            response.Organization = null; // Şirketi yok
        }

        private async Task LoadFreelancerProfile(MyProfileDto response, Guid freelancerId, CancellationToken token)
        {
            var freelancer = await _freelancerRepo.GetByIdAsync(freelancerId, token);
            if (freelancer == null) throw new Exception("Freelancer profili bulunamadı.");

            response.FullName = freelancer.FullName;
            response.Roles = new List<string> { "Freelancer" };
            response.Phone = freelancer.Phone;
            response.Organization = null; // Şirketi yok (Kendisi şirket gibidir ama yapı farklı)
        }
    }
}
