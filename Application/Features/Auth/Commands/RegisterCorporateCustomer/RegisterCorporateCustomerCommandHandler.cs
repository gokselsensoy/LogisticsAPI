using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Features.Auth.Commands.RegisterSupplier;
using Domain.Entities;
using Domain.Entities.Companies;
using Domain.Entities.Customers;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterCorporateCustomer
{
    public class RegisterCorporateCustomerCommandHandler : IRequestHandler<RegisterCorporateCustomerCommand, Guid>
    {
        private IIdentityService _identityService;
        private IUserRepository _userRepository;
        private ICustomerRepository _customerRepository;
        private ICorporateResponsibleRepository _corporateResponsibleRepository;
        private IUnitOfWork _unitOfWork;

        public RegisterCorporateCustomerCommandHandler(
            IIdentityService identityService, 
            IUserRepository userRepository, 
            ICustomerRepository customerRepository,
            ICorporateResponsibleRepository corporateResponsibleRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _corporateResponsibleRepository = corporateResponsibleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterCorporateCustomerCommand request, CancellationToken token)
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
                await _identityService.AddToRoleAsync(identityId, "CorporateCustomer", token);

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
                var newIdentityId = await _identityService.CreateUserAsync(request.Email, request.Password, "CorporateCustomer", token);
                identityId = newIdentityId.Value;

                var newAppUser = AppUser.Create(identityId, request.Email);
                _userRepository.Add(newAppUser);
                appUserId = newAppUser.Id;
            }

            // 2. Şirketi (Corporate Customer) Oluştur
            var corporateCustomer = new CorporateCustomer(
                request.CorporateName, request.CvrNumber, request.Email, request.Phone);

            var responsible = corporateCustomer.CreateResponsible(
            appUserId,
            request.FullName,
            request.Phone,
            new List<CorporateRole> { CorporateRole.Admin } // İlk kaydolan kişi Admin olur
        );

            // Sorumluyu kendi reposuna ekle (Çünkü artık Aggregate Root)
            _corporateResponsibleRepository.Add(responsible);

            _customerRepository.Add(corporateCustomer);
            await _unitOfWork.SaveChangesAsync(token);
            return corporateCustomer.Id;
        }
    }
}
