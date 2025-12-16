using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Company;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Events.WorkerEvents;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Workers.Commands.CreateWorker
{
    public class CreateWorkerCommandHandler : IRequestHandler<CreateWorkerCommand, Guid>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICompanyRepository _companyRepository;
        private readonly IWorkerRepository _workerRepo;
        private readonly IUserRepository _userRepo;
        private readonly IIdentityService _identityService;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkerCommandHandler(
            ICurrentUserService currentUser,
            ICompanyRepository companyRepository,
            IWorkerRepository workerRepo,
            IUserRepository userRepo,
            IIdentityService identityService,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _companyRepository = companyRepository;
            _workerRepo = workerRepo;
            _userRepo = userRepo;
            _identityService = identityService;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateWorkerCommand request, CancellationToken token)
        {
            // 1. Yetki Kontrolü (İşlemi yapan Admin mi?)
            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Bu işlem için bir şirket profili ile giriş yapmalısınız.");

            if (!_currentUser.Roles.Contains("Admin")) // Rol string olarak geliyorsa
                throw new UnauthorizedAccessException("Personel ekleme yetkiniz yok.");

            Guid appUserId;
            Guid identityId;

            var existingIdentityUser = await _identityService.GetByEmailAsync(request.Email, token);

            if (existingIdentityUser != null)
            {
                // SENARYO 1: Kullanıcı Zaten Var
                identityId = existingIdentityUser.Id;

                // Bizdeki AppUser karşılığını bul
                var existingAppUser = await _userRepo.GetByIdentityIdAsync(identityId, token);
                if (existingAppUser == null) throw new Exception("Veri tutarsızlığı: Identity var ama AppUser yok.");

                appUserId = existingAppUser.Id;

                // Not: IdentityService'e "HasRole" sorabilirsin veya direkt "AddToRole" çağırabilirsin (Idempotent ise).
                // Biz güvenli tarafta olup ekleyelim (IdentityAPI zaten varsa hata vermemeli veya kontrol etmeli)
                var company = await _companyRepository.GetByIdAsync(_currentUser.CompanyId.Value);
                if (company == null) throw new Exception("Şirket bulunamadı.");

                string roleToAssign = company switch
                {
                    Transporter => "Transporter",
                    Supplier => "Supplier",
                    _ => throw new Exception("Geçersiz şirket tipi.")
                };

                // O rolü kullanıcıya ekle
                await _identityService.AddToRoleAsync(identityId, roleToAssign, token);
            }
            else
            {
                var company = await _companyRepository.GetByIdAsync(_currentUser.CompanyId.Value);
                if (company == null) throw new Exception("Şirket bulunamadı.");

                string roleToAssign = company switch
                {
                    Transporter => "Transporter",
                    Supplier => "Supplier",
                    _ => throw new Exception("Geçersiz şirket tipi.")
                };

                // SENARYO 2: Kullanıcı Yok, Sıfırdan Oluştur
                var newIdentityId = await _identityService.CreateUserAsync(request.Email, request.Password, roleToAssign, token);
                if (newIdentityId == null) throw new Exception("Kullanıcı oluşturulamadı.");

                identityId = newIdentityId.Value;

                // AppUser oluştur (Artık isim/tel yok, sadece email ve ID)
                var newAppUser = AppUser.Create(identityId, request.Email);
                _userRepo.Add(newAppUser);

                appUserId = newAppUser.Id;
            }

            var worker = new Worker(
                _currentUser.CompanyId.Value,
                request.DepartmentId,
                appUserId,      // Bulduğumuz veya Yarattığımız ID
                request.FullName, // <-- Worker'a özel isim
                request.Phone,    // <-- Worker'a özel telefon
                request.Roles
                );

            worker.AddDomainEvent(new WorkerCreatedEvent(worker.Id, request.Email, request.FullName, request.Phone, request.Password));

            _workerRepo.Add(worker);
            await _unitOfWork.SaveChangesAsync(token);

            return worker.Id;
        }
    }
}
