using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Customer;
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
        private IUnitOfWork _unitOfWork;

        public RegisterCorporateCustomerCommandHandler(
            IIdentityService identityService, 
            IUserRepository userRepository, 
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterCorporateCustomerCommand request, CancellationToken token)
        {
            var identityId = await _identityService.CreateUserAsync(
                request.Email, request.Password, "CorporateCustomer", token);

            var localUser = AppUser.Create(identityId.Value, request.Email, request.Phone, request.FullName);
            _userRepository.Add(localUser);

            var corporateCustomer = new CorporateCustomer(
                request.CorporateName, request.CvrNumber, request.Email, request.Phone);

            corporateCustomer.AddResponsible(localUser.Id, CorporateRole.Admin);

            _customerRepository.Add(corporateCustomer);
            await _unitOfWork.SaveChangesAsync(token);

            return corporateCustomer.Id;
        }
    }
}
