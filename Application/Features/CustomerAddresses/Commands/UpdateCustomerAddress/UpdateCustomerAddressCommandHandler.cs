using Application.Abstractions.Services;
using Domain.Entities.Customer;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Features.CustomerAddresses.Commands.UpdateCustomerAddress
{
    public class UpdateCustomerAddressCommandHandler : IRequestHandler<UpdateCustomerAddressCommand, Unit>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICorporateResponsibleRepository _responsibleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory;

        public UpdateCustomerAddressCommandHandler(ICustomerRepository customerRepo, ICorporateResponsibleRepository responsibleRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _customerRepo = customerRepo;
            _responsibleRepo = responsibleRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task<Unit> Handle(UpdateCustomerAddressCommand request, CancellationToken token)
        {
            Customer? customer = null;
            bool isAuthorized = false;

            // 1. Individual Logic
            if (_currentUser.ProfileType == "IndividualCustomer")
            {
                var indCustomer = await _customerRepo.GetByAppUserIdAsync(_currentUser.AppUserId, token);
                if (indCustomer != null)
                {
                    customer = await _customerRepo.GetByIdWithAddressesAsync(indCustomer.Id, token);
                    // Adres bu müşteriye mi ait?
                    if (customer != null && customer.Addresses.Any(a => a.Id == request.AddressId))
                    {
                        isAuthorized = true;
                    }
                }
            }
            // 2. Corporate Logic
            else if (_currentUser.ProfileType == "CorporateResponsible")
            {
                var responsible = await _responsibleRepo.GetByIdWithAssignmentsAsync(_currentUser.ProfileId.Value, token);
                // Not: ProfileId kullanmak daha güvenli (Handlerda _currentUserService'e ProfileId eklemiştik)

                if (responsible != null)
                {
                    customer = await _customerRepo.GetByIdWithAddressesAsync(responsible.CorporateCustomerId, token);

                    // Adres şirkete ait mi?
                    bool addressBelongsToCompany = customer.Addresses.Any(a => a.Id == request.AddressId);

                    if (addressBelongsToCompany)
                    {
                        // A. Admin ise her şeye yetkisi var
                        if (responsible.IsAdmin())
                        {
                            isAuthorized = true;
                        }
                        // B. Standart User ise atama tablosuna bak (CorporateAddressResponsibleMap)
                        else if (responsible.AssignedAddresses.Any(map => map.AddressId == request.AddressId))
                        {
                            isAuthorized = true;
                        }
                    }
                }
            }

            if (!isAuthorized || customer == null)
                throw new UnauthorizedAccessException("Bu adresi güncelleme yetkiniz yok.");

            // 3. Güncelle
            var location = _geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));
            var addressVo = new Address(request.Street, request.BuildingNo, request.ZipCode, request.City, request.Country, location, request.Floor, request.Door);

            customer.UpdateAddress(request.AddressId, request.Title, addressVo, request.Type);

            _customerRepo.Update(customer);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
