using Application.Abstractions.Services;
using Domain.Entities.Customer;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Features.CustomerAddresses.Commands.AddCustomerAddress
{
    public class AddCustomerAddressCommandHandler : IRequestHandler<AddCustomerAddressCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICorporateResponsibleRepository _responsibleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory;

        public AddCustomerAddressCommandHandler(ICustomerRepository customerRepo, ICorporateResponsibleRepository responsibleRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _customerRepo = customerRepo;
            _responsibleRepo = responsibleRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task<Guid> Handle(AddCustomerAddressCommand request, CancellationToken token)
        {
            Customer? customer = null;
            Guid actingUserId = _currentUser.AppUserId;

            // 1. Profil Tipi Nedir?
            if (_currentUser.ProfileType == "IndividualCustomer")
            {
                customer = await _customerRepo.GetByAppUserIdAsync(actingUserId, token);
            }
            else if (_currentUser.ProfileType == "CorporateResponsible")
            {
                var responsible = await _responsibleRepo.GetByAppUserIdAsync(actingUserId, token);
                if (responsible == null) throw new UnauthorizedAccessException("Sorumlu bulunamadı.");

                // Eğer Corporate User ise (Admin değilse), adres EKLEME yetkisi var mıdır?
                // Genelde Admin ekler, User yönetir. Ama biz burada Admin ise ekleyebilsin diyelim.
                if (!responsible.IsAdmin())
                    throw new UnauthorizedAccessException("Sadece Adminler yeni adres tanımlayabilir.");

                customer = await _customerRepo.GetByIdWithAddressesAsync(responsible.CorporateCustomerId, token);
            }

            if (customer == null) throw new Exception("Müşteri bulunamadı.");

            // 2. Value Object Dönüşümü
            var location = _geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));
            var addressVo = new Address(request.Street, request.BuildingNo, request.ZipCode, request.City, request.Country, location, request.Floor, request.Door);

            // 3. Ekle
            var newAddress = customer.AddAddress(request.Title, addressVo, request.Type);

            _customerRepo.Update(customer);
            await _unitOfWork.SaveChangesAsync(token);

            return newAddress.Id;
        }
    }
}
