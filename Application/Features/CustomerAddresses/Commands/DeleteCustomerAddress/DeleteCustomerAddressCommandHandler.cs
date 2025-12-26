using Application.Abstractions.Services;
using Domain.Entities.Customer;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.CustomerAddresses.Commands.DeleteCustomerAddress
{
    public class DeleteCustomerAddressCommandHandler : IRequestHandler<DeleteCustomerAddressCommand, Unit>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICorporateResponsibleRepository _responsibleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCustomerAddressCommandHandler(
            ICustomerRepository customerRepo,
            ICorporateResponsibleRepository responsibleRepo,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _customerRepo = customerRepo;
            _responsibleRepo = responsibleRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteCustomerAddressCommand request, CancellationToken token)
        {
            Customer? customer = null;

            // 1. Yetki Kontrolü ve Müşteriyi Bulma
            if (_currentUser.ProfileType == "IndividualCustomer")
            {
                var indCustomer = await _customerRepo.GetByAppUserIdAsync(_currentUser.UserId, token);
                if (indCustomer != null)
                {
                    customer = await _customerRepo.GetByIdWithAddressesAsync(indCustomer.Id, token);
                }
            }
            else if (_currentUser.ProfileType == "CorporateResponsible")
            {
                var responsible = await _responsibleRepo.GetByAppUserIdAsync(_currentUser.UserId, token);

                // Sadece Adminler adres silebilir mi? (Genelde evet)
                if (responsible != null && responsible.IsAdmin())
                {
                    customer = await _customerRepo.GetByIdWithAddressesAsync(responsible.CorporateCustomerId, token);
                }
            }

            if (customer == null) throw new UnauthorizedAccessException("Adres silme yetkiniz yok.");

            // 2. Adres Gerçekten Bu Müşteriye mi Ait?
            var addressExists = customer.Addresses.Any(a => a.Id == request.AddressId);
            if (!addressExists) throw new Exception("Adres bulunamadı veya size ait değil.");

            // 3. Silme (Soft Delete)
            // Aggregate Root üzerinden siliyoruz
            customer.RemoveAddress(request.AddressId);

            _customerRepo.Update(customer);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
