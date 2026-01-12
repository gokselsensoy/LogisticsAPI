using Application.Abstractions.EntityRepositories;
using Application.Features.Suppliers.DTOs;
using Application.Shared.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities.Departments;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Infrastructure.Persistence.QueryRepositories
{
    public class SupplierQueryRepository : ISupplierQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SupplierQueryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<SupplierDto>> GetNearbySuppliersAsync(
            double lat, double lon, string? searchText, int page, int size, CancellationToken token)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var userLocation = geometryFactory.CreatePoint(new Coordinate(lon, lat));

            // Sorguyu oluşturuyoruz (Henüz DB'ye gitmedi)
            var query = _context.Set<Terminal>()
                .AsNoTracking()
                .Where(t => !t.IsDeleted && t.ServiceRadiusKm.HasValue)
                .Where(t => t.Address.Location.Distance(userLocation) <= (t.ServiceRadiusKm * 1000))
                .Select(t => t.Department.Company); // Supplier'a geçiş

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(s => s.Name.Contains(searchText));
            }

            // Distinct önemli (Aynı supplier'ın 2 terminali de kapsıyor olabilir)
            var distinctQuery = query.Distinct();

            var totalCount = await distinctQuery.CountAsync(token);

            var suppliers = await distinctQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider) // <-- AUTOMAPPER
                .ToListAsync(token);

            return new PaginatedResponse<SupplierDto>(suppliers, totalCount, page, size);
        }
    }
}
