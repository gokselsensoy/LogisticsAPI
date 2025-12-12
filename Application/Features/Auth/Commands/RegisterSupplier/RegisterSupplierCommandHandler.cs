using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Company;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterSupplier
{
    public class RegisterSupplierCommandHandler : IRequestHandler<RegisterSupplierCommand, Guid>
    {
        private readonly IIdentityService _identityService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterSupplierCommandHandler(
            IIdentityService identityService,
            ISupplierRepository supplierRepository,
            IDepartmentRepository departmentRepository,
            IWorkerRepository workerRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _supplierRepository = supplierRepository;
            _departmentRepository = departmentRepository;
            _workerRepository = workerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterSupplierCommand request, CancellationToken token)
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

                // Rol Ekle: Adamın "Supplier" rolü yoksa ekle
                await _identityService.AddToRoleAsync(identityId, "Supplier", token);

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
                var newIdentityId = await _identityService.CreateUserAsync(request.Email, request.Password, "Supplier", token);
                identityId = newIdentityId.Value;

                var newAppUser = AppUser.Create(identityId, request.Email);
                _userRepository.Add(newAppUser);
                appUserId = newAppUser.Id;
            }

            // 2. Şirketi (Supplier) Oluştur
            var supplier = new Supplier(request.CompanyName, request.CvrNumber, request.Email);
            _supplierRepository.Add(supplier);

            var defaultDept = supplier.CreateDefaultDepartment();
            _departmentRepository.Add(defaultDept); // Departman Repository'sine ekle

            // 3. Worker (Owner) Oluştur (FACTORY METHOD KULLANIMI)
            // DİKKAT: new Worker(...) kullanmıyoruz. supplier.CreateWorker(...) kullanıyoruz.
            var worker = supplier.CreateWorker(
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
            await _unitOfWork.SaveChangesAsync(token);

            return supplier.Id;
        }
    }
}
