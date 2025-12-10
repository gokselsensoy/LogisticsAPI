using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Company;
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
            // 1. Identity
            var identityId = await _identityService.CreateUserAsync(
                request.Email, request.Password, "Freelancer", token);

            // 2. Local User
            var localUser = AppUser.Create(identityId.Value, request.Email, request.Phone, request.FullName);
            _userRepository.Add(localUser);

            // 3. Freelancer Entity
            var freelancer = new Freelancer(
                localUser.Id,
                request.FullName,
                request.CvrNumber,
                request.Email
            );

            // 4. Kaydet
            _freelancerRepository.Add(freelancer);
            await _unitOfWork.SaveChangesAsync(token);

            return freelancer.Id;
        }
    }
}
