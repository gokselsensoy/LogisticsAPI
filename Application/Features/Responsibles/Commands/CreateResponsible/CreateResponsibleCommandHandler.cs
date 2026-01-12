using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Customers;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Responsibles.Commands.CreateResponsible
{
    public class CreateResponsibleCommandHandler : IRequestHandler<CreateResponsibleCommand, Guid>
    {
        private readonly ICorporateResponsibleRepository _responsibleRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepo; // AppUser repository'si (Varsayıyorum)
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CreateResponsibleCommandHandler(
            ICorporateResponsibleRepository responsibleRepo,
            ICustomerRepository customerRepo,
            IIdentityService identityService,
            IUserRepository userRepo,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _responsibleRepo = responsibleRepo;
            _customerRepo = customerRepo;
            _identityService = identityService;
            _userRepo = userRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateResponsibleCommand request, CancellationToken token)
        {
            // 1. İşlemi Yapanın Yetkisini Kontrol Et (Admin mi?)
            // Login olan Responsible'ı bul
            var currentResponsible = await _responsibleRepo.GetByAppUserIdAsync(_currentUser.AppUserId, token);

            if (currentResponsible == null || !currentResponsible.IsAdmin())
                throw new UnauthorizedAccessException("Sorumlu ekleme yetkiniz yok.");

            // 2. Identity ve AppUser İşlemleri (Worker mantığıyla aynı)
            Guid appUserId;
            Guid identityId;

            // Email ile IdentityUser var mı?
            var existingIdentityUser = await _identityService.GetByEmailAsync(request.Email, token);

            string roleToAssign = "CorporateCustomer"; // Veya "CorporateResponsible" (Sabit bir rol adı)

            if (existingIdentityUser != null)
            {
                // SENARYO 1: Kullanıcı Zaten Var
                identityId = existingIdentityUser.Id;

                var existingAppUser = await _userRepo.GetByIdentityIdAsync(identityId, token);
                if (existingAppUser == null) throw new Exception("Veri tutarsızlığı: Identity var ama AppUser yok.");

                appUserId = existingAppUser.Id;

                // Rolü ekle (Eğer zaten varsa IdentityService bunu yönetmeli)
                await _identityService.AddToRoleAsync(identityId, roleToAssign, token);
            }
            else
            {
                // SENARYO 2: Kullanıcı Yok, Sıfırdan Oluştur
                // Password fronttan geliyorsa onu kullan, yoksa geçici şifre üret
                var newIdentityId = await _identityService.CreateUserAsync(request.Email, request.Password, roleToAssign, token);
                if (newIdentityId == null) throw new Exception("Kullanıcı oluşturulamadı.");

                identityId = newIdentityId.Value;

                // AppUser oluştur
                var newAppUser = AppUser.Create(identityId, request.Email);
                _userRepo.Add(newAppUser);
                // Burada SaveChanges çağırmazsan FK hatası alabilirsin, UnitOfWork transaction içindeyse sorun yok.
                // Ama garanti olsun diye UserRepo'nun db context'i ile aynı transactiondaysak sorun olmaz.

                appUserId = newAppUser.Id;
            }

            // 3. Şirketi Bul
            var corporate = await _customerRepo.GetByIdAsync(currentResponsible.CorporateCustomerId, token) as CorporateCustomer;
            if (corporate == null) throw new Exception("Şirket bulunamadı.");

            // 4. Responsible Entity'sini Oluştur
            // Factory metodu kullanarak (Aggregate üzerinden)
            var newResponsible = corporate.CreateResponsible(
                appUserId,
                request.FullName,
                request.Phone,
                request.Roles // CorporateRole enum listesi
            );

            // 5. Kaydet
            _responsibleRepo.Add(newResponsible);
            await _unitOfWork.SaveChangesAsync(token);

            return newResponsible.Id;
        }
    }
}
