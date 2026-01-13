using Application.Abstractions.EntityRepositories;
using Application.Abstractions.Services;
using Application.Features.Products.DTOs;
using Application.Shared.Pagination;
using Domain.Entities.Companies;
using Domain.Entities.Departments;
using Domain.Entities.Inventories;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Application.Features.Products.Queries
{
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PaginatedResponse<ProductListingDto>>
    {
        private readonly IProductQueryRepository _productQueryRepo;

        public SearchProductsQueryHandler(IProductQueryRepository productQueryRepo)
        {
            _productQueryRepo = productQueryRepo;
        }

        public async Task<PaginatedResponse<ProductListingDto>> Handle(SearchProductsQuery request, CancellationToken token)
        {
            return await _productQueryRepo.SearchProductsByAddressAsync(
                request.AddressId,
                request.SearchText,
                request.Category,
                request.SupplierId,
                request.PageNumber,
                request.PageSize,
                token
            );
        }
    }
}
