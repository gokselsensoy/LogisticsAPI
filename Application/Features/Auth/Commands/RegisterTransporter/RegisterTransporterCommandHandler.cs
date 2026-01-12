using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
<<<<<<< HEAD
using Domain.Entities.Companies;
using Domain.Entities.Departments;
=======
using Domain.Entities.Company;
>>>>>>> 4952e842f75477b0a804ce86a86615304aac01d9
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;
using NewMultilloApi.Application.DTOs.SubOrbit;

namespace Application.Features.Auth.Commands.RegisterTransporter
{
    public class RegisterTransporterCommandHandler : IRequestHandler<RegisterTransporterCommand, InitiateSubscriptionResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITransporterRepository _transporterRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubOrbitService _subOrbitService;

        public RegisterTransporterCommandHandler(
            IIdentityService identityService,
            ITransporterRepository transporterRepository,
            IDepartmentRepository departmentRepository,
            IWorkerRepository workerRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ISubOrbitService subOrbitService)
        {
            _identityService = identityService;
            _transporterRepository = transporterRepository;
            _departmentRepository = departmentRepository;
            _workerRepository = workerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _subOrbitService = subOrbitService;
        }

        public async Task<InitiateSubscriptionResponse> Handle(RegisterTransporterCommand request, CancellationToken token)
        {
            Guid appUserId;
            Guid identityId;

            // 1. KULLANICI KONTROLÜ
            var existingIdentityUser = await _identityService.GetByEmailAsync(request.Email, token);

            if (existingIdentityUser != null)
            {
                // Kullanıcı var -> Mevcut ID'yi kullan
                identityId = existingIdentityUser.Id;

                var existingAppUser = await _userRepository.GetByIdentityIdAsync(identityId, token);
                appUserId = existingAppUser.Id;

                // Rol Ekle: Adamın "Transporter" rolü yoksa ekle
                await _identityService.AddToRoleAsync(identityId, "Transporter", token);
                
                

                // GÜVENLİK NOTU:
                // Public bir kayıt ekranında (Login olmadan), birisi başkasının emailini girip şirket kurabilir mi?
                // Normalde burada şifre kontrolü yapılır veya "Email zaten var, lütfen giriş yapıp şirket ekleyin" denir.
                // Ancak senin isteğin üzerine "Direkt Bağla" yapıyoruz. 
                // Eğer request.Password geliyorsa, IdentityService.CheckPassword ile doğrulama yapman iyi olur.
                // TO DO: BU SEBEPLE BURAYA EMAİL CONFİRMATİON TARZI BİR ŞEY GELECEK
            }
            else
            {
                // Kullanıcı yok -> Yarat
                var newIdentityId = await _identityService.CreateUserAsync(request.Email, request.Password, "Transporter", token);
                identityId = newIdentityId.Value;

                var newAppUser = AppUser.Create(identityId, request.Email);
                _userRepository.Add(newAppUser);
                appUserId = newAppUser.Id;
            }

            // 2. Şirketi (Transporter) Oluştur
            var transporter = new Transporter(request.CompanyName, request.CvrNumber, request.Email);
            _transporterRepository.Add(transporter);

            var defaultDept = transporter.CreateDefaultDepartment();
            _departmentRepository.Add(defaultDept); // Departman Repository'sine ekle

            // 3. Worker (Owner) Oluştur (FACTORY METHOD KULLANIMI)
            // DİKKAT: new Worker(...) kullanmıyoruz. supplier.CreateWorker(...) kullanıyoruz.
            var worker = transporter.CreateWorker(
                appUserId,
                request.FullName,
                request.Phone,
                new List<WorkerRole> { WorkerRole.Admin }
            );

            // Worker'ın departmanını ayarla (Varsayılan departman)
            worker.ChangeDepartment(defaultDept.Id);

            // 4. Worker'ı Repository'e Ekle
            // Worker kendi başına bir Aggregate Root olduğu için kendi reposuna eklenir.
            _workerRepository.Add(worker);

            // 5. Hepsini Tek Seferde Kaydet
            request.initiateSubscriptionDto.PayerExternalId = appUserId.ToString();
            var res = await _subOrbitService.InitiateCheckoutAsync(request.initiateSubscriptionDto);
            await _unitOfWork.SaveChangesAsync(token);
            return res;
        }
    }
}
