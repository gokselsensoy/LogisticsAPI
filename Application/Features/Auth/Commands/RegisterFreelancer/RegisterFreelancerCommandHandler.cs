using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Companies;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterFreelancer
{
    public class RegisterFreelancerCommandHandler : IRequestHandler<RegisterFreelancerCommand, Guid>
    {
        private IIdentityService _identityService;
        private IUserRepository _userRepository;
        private IFreelancerRepository _freelancerRepository;
        private IUnitOfWork _unitOfWork;

        public RegisterFreelancerCommandHandler(
            IIdentityService identityService,
            IUserRepository userRepository,
            IFreelancerRepository freelancerRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _freelancerRepository = freelancerRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(RegisterFreelancerCommand request, CancellationToken token)
        {
            Guid appUserId;

            // 1. Kullanıcı Kontrolü
            var existingUser = await _identityService.GetByEmailAsync(request.Email, token);

            if (existingUser != null)
            {
                var appUser = await _userRepository.GetByIdentityIdAsync(existingUser.Id, token);
                appUserId = appUser.Id;
                await _identityService.AddToRoleAsync(existingUser.Id, "Freelancer", token);
            }
            else
            {
                var newId = await _identityService.CreateUserAsync(request.Email, request.Password, "Freelancer", token);
                var appUser = AppUser.Create(newId.Value, request.Email);
                _userRepository.Add(appUser);
                appUserId = appUser.Id;
            }

            // 2. Freelancer Profilini Oluştur
            // Burada da İsim ve Telefon Command'den geliyor
            var freelancer = new Freelancer(
                appUserId,
                request.FullName, // <-- Freelancer tablosuna yazılacak isim
                request.Phone, // <-- Freelancer tablosuna yazılacak tel
                request.CvrNumber,
                request.Email
            );

            _freelancerRepository.Add(freelancer);
            await _unitOfWork.SaveChangesAsync(token);

            return freelancer.Id;
        }
    }
}
