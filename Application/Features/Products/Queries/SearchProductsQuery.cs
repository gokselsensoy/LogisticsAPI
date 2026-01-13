using Application.Features.CustomerAddresses.DTOs;
using Application.Features.CustomerAddresses.Queries.GetMyAddresses;
using Application.Features.Products.DTOs;
using Application.Shared.Pagination;
using Domain.Enums;
using MediatR;

namespace Application.Features.Products.Queries
{
    public class SearchProductsQuery : PaginatedRequest, IRequest<PaginatedResponse<ProductListingDto>>
    {
        public Guid AddressId { get; set; }

        // Filtreler
        public string? SearchText { get; set; } // Ürün adı veya Supplier adı
        public ProductCategory? Category { get; set; }
        public Guid? SupplierId { get; set; } // Eğer spesifik bir suppliera tıklandıysa
    }
}
