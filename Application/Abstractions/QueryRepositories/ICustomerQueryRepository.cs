using Application.Features.CustomerAddresses.DTOs;

namespace Application.Abstractions.EntityRepositories
{
    public interface ICustomerQueryRepository
    {
        Task<List<AddressSelectionDto>> GetAddressesForIndividualAsync(Guid customerId, CancellationToken token);
        Task<List<AddressSelectionDto>> GetAddressesForResponsibleAsync(Guid responsibleId, CancellationToken token);
    }
}
