using Application.Features.Suppliers.DTOs;
using Application.Shared.Pagination;

namespace Application.Abstractions.EntityRepositories
{
    public interface ISupplierQueryRepository
    {
        Task<PaginatedResponse<SupplierDto>> GetNearbySuppliersByAddressAsync(
        Guid addressId,
        string? searchText,
        int page,
        int size,
        CancellationToken token);
    }
}
