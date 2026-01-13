using Application.Features.Products.DTOs;
using Application.Shared.Pagination;
using Domain.Enums;

namespace Application.Abstractions.EntityRepositories
{
    public interface IProductQueryRepository
    {
        Task<PaginatedResponse<ProductListingDto>> SearchProductsByAddressAsync(
        Guid addressId,
        string? searchText,
        ProductCategory? category,
        Guid? supplierId,
        int page,
        int size,
        CancellationToken token);
    }
}
