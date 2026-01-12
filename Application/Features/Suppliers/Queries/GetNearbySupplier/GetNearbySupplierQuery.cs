using Application.Features.CustomerAddresses.DTOs;
using Application.Features.CustomerAddresses.Queries.GetMyAddresses;
using Application.Features.Suppliers.DTOs;
using Application.Shared.Pagination;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetNearbySupplier
{
    public class GetNearbySuppliersQuery : PaginatedRequest, IRequest<PaginatedResponse<SupplierDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? SearchText { get; set; }
    }
}
