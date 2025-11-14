using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Users.Commands.RegisterUser
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IIdentityService _identityService; // IdentityAPI ile konuşur
        private readonly IUserRepository _userRepository; // Kendi DB'mize yazar
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCustomerCommandHandler(
            IIdentityService identityService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Adım A: IdentityAPI'ye S2S çağrısı yap
            Guid? identityId = await _identityService.CreateWorkerUserAsync(
                request.Email,
                request.Password,
                //"Customer", // Role
                cancellationToken);

            // Adım B: Kimlik al
            if (identityId == null)
            {
                // IdentityAPI kullanıcıyı oluşturamadı (örn: email zaten var)
                throw new Exception("Kimlik oluşturulamadı. Email zaten kullanılıyor olabilir.");
            }

            // Adım C: İş verisini (Customer) oluştur
            var newCustomer = User.Create(
                identityId.Value,
                request.Email,
                request.PhoneNumber);

            // Adım D: Kendi DB'mize kaydet
            _userRepository.Add(newCustomer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newCustomer.Id;
        }
    }
}
