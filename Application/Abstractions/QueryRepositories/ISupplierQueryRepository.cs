using Application.Features.Suppliers.DTOs;
using Application.Shared.Pagination;

namespace Application.Abstractions.EntityRepositories
{
    public interface ISupplierQueryRepository
    {
        Task<PaginatedResponse<SupplierDto>> GetNearbySuppliersAsync(double lat, double lon, string? searchText, int page, int size, CancellationToken token);
        
    }


}
