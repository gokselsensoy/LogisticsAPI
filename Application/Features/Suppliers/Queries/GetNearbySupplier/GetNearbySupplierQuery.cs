using Application.Features.CustomerAddresses.DTOs;
using Application.Features.CustomerAddresses.Queries.GetMyAddresses;
using Application.Features.Suppliers.DTOs;
using Application.Shared.Pagination;
using Application.Shared.ResultModels;
using MediatR;

namespace Application.Features.Suppliers.Queries.GetNearbySupplier
{
    public class GetNearbySuppliersQuery : PaginatedRequest, IRequest<PaginatedResponse<SupplierDto>>
    {
        public Guid AddressId { get; set; } // Lat/Lon yerine bu geldi
        public string? SearchText { get; set; }
    }
}
