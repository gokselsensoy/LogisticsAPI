using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Customer;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterIndividualCustomer
{
    public class RegisterIndividualCustomerCommandHandler : IRequestHandler<RegisterIndividualCustomerCommand, Guid>
    {
        private readonly IIdentityService _identityService;
        private readonly IRepository<IndividualCustomer> _customerRepository;
        private readonly IRepository<AppUser> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterIndividualCustomerCommandHandler(
            IIdentityService identityService,
            IRepository<IndividualCustomer> customerRepository,
            IRepository<AppUser> userRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterIndividualCustomerCommand request, CancellationToken token)
        {
            var identityId = await _identityService.CreateUserAsync(
                request.Email,
                request.Password,
                "IndividualCustomer",
                token);

            if (identityId == null)
                throw new Exception("Kimlik oluşturulamadı.");

            var localUser = AppUser.Create(
                identityId.Value,
                request.Email,
                request.Phone,
                request.FullName
            );

            _userRepository.Add(localUser);

            var individualCustomer = new IndividualCustomer(
                request.FullName,
                localUser.Id,
                request.Email,
                request.Phone
            );

            _customerRepository.Add(individualCustomer);
            await _unitOfWork.SaveChangesAsync(token);

            return individualCustomer.Id;
        }
    }
}
