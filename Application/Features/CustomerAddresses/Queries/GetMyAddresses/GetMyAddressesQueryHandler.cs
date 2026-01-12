using Application.Abstractions.EntityRepositories;
using Application.Abstractions.Services;
using Application.Features.CustomerAddresses.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.CustomerAddresses.Queries.GetMyAddresses
{
    public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, List<AddressSelectionDto>>
    {
        private readonly ICustomerQueryRepository _customerQueryRepo;
        private readonly ICurrentUserService _currentUser;

        public GetMyAddressesQueryHandler(ICustomerQueryRepository customerQueryRepo, ICurrentUserService currentUser)
        {
            _customerQueryRepo = customerQueryRepo;
            _currentUser = currentUser;
        }

        public async Task<List<AddressSelectionDto>> Handle(GetMyAddressesQuery request, CancellationToken token)
        {
            // 1. Kullanıcı bilgilerini al
            if (!_currentUser.ProfileId.HasValue)
                return new List<AddressSelectionDto>();

            var profileId = _currentUser.ProfileId.Value;
            var profileType = _currentUser.ProfileType; // "IndividualCustomer" veya "CorporateResponsible"

            // 2. Profil tipine göre yönlendir
            if (profileType == "IndividualCustomer")
            {
                // Bireysel müşteri logic'i
                return await _customerQueryRepo.GetAddressesForIndividualAsync(profileId, token);
            }
            else if (profileType == "CorporateResponsible") // Veya "Worker" vb.
            {
                // Kurumsal sorumlu logic'i (Admin kontrolü repository içinde)
                return await _customerQueryRepo.GetAddressesForResponsibleAsync(profileId, token);
            }

            return new List<AddressSelectionDto>();
        }
    }
}
