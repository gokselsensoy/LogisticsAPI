using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Companies;
using Domain.Entities.Customers;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterIndividualCustomer
{
    public class RegisterIndividualCustomerCommandHandler : IRequestHandler<RegisterIndividualCustomerCommand, Guid>
    {
        private readonly IIdentityService _identityService;
        private readonly IIndividualCustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterIndividualCustomerCommandHandler(
            IIdentityService identityService,
            IIndividualCustomerRepository customerRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterIndividualCustomerCommand request, CancellationToken token)
        {
            Guid appUserId;

            var existingUser = await _identityService.GetByEmailAsync(request.Email, token);

            if (existingUser != null)
            {
                var appUser = await _userRepository.GetByIdentityIdAsync(existingUser.Id, token);
                appUserId = appUser.Id;
                await _identityService.AddToRoleAsync(existingUser.Id, "IndividualCustomer", token);
            }
            else
            {
                var newId = await _identityService.CreateUserAsync(request.Email, request.Password, "IndividualCustomer", token);
                var appUser = AppUser.Create(newId.Value, request.Email);
                _userRepository.Add(appUser);
                appUserId = appUser.Id;
            }

            var individualCustomer = new IndividualCustomer(
                appUserId,
                request.FullName,
                request.Email,
                request.Phone
            );

            _customerRepository.Add(individualCustomer);
            await _unitOfWork.SaveChangesAsync(token);

            return individualCustomer.Id;
        }
    }
}
