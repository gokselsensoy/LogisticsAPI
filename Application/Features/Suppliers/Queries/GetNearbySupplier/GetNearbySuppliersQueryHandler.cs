using Application.Abstractions.EntityRepositories;
using Application.Abstractions.Services;
using Application.Features.Suppliers.DTOs;
using Application.Shared.Pagination;
using Application.Shared.ResultModels;
using Domain.Entities.Departments;
using Domain.Repositories;
using MediatR;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Application.Features.Suppliers.Queries.GetNearbySupplier
{
    public class GetNearbySuppliersQueryHandler : IRequestHandler<GetNearbySuppliersQuery, PaginatedResponse<SupplierDto>>
    {
        private readonly ISupplierQueryRepository _supplierQueryRepo;

        public GetNearbySuppliersQueryHandler(ISupplierQueryRepository supplierQueryRepo)
        {
            _supplierQueryRepo = supplierQueryRepo;
        }

        public async Task<PaginatedResponse<SupplierDto>> Handle(GetNearbySuppliersQuery request, CancellationToken token)
        {
            var paginatedData = await _supplierQueryRepo.GetNearbySuppliersByAddressAsync(
                request.AddressId,
                request.SearchText,
                request.PageNumber,
                request.PageSize,
                token
            );

            return paginatedData;
        }
    }
}
