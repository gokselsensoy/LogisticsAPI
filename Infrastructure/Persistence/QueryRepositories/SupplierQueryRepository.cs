using Application.Abstractions.EntityRepositories;
using Application.Features.Suppliers.DTOs;
using Application.Shared.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities.Companies;
using Domain.Entities.Customers;
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

        public async Task<PaginatedResponse<SupplierDto>> GetNearbySuppliersByAddressAsync(
    Guid addressId, string? searchText, int page, int size, CancellationToken token)
        {
            // 1. ADIM: Lokasyonu çek (Aynı kalıyor)
            var userLocation = await _context.Set<CustomerAddress>()
                .AsNoTracking()
                .Where(a => a.Id == addressId)
                .Select(a => a.Address.Location)
                .FirstOrDefaultAsync(token);

            if (userLocation == null)
            {
                return new PaginatedResponse<SupplierDto>(new List<SupplierDto>(), 0, page, size);
            }

            // 2. ADIM: Query (OfType eklenmiş hali)
            var query = _context.Set<Terminal>()
                .AsNoTracking()
                .Where(t => !t.IsDeleted && t.ServiceRadiusKm.HasValue)
                .Where(t => t.Address.Location.Distance(userLocation) <= (t.ServiceRadiusKm * 1000))
                // BURAYA KADAR AYNI

                // ÖNEMLİ KISIM:
                .Select(t => t.Department.Company) // Şu an elimizde IQueryable<Company> var
                .OfType<Supplier>();               // ARTIK ELİMİZDE IQueryable<Supplier> VAR!

            if (!string.IsNullOrEmpty(searchText))
            {
                // Artık 's' bir Supplier olduğu için hem Company hem Supplier alanlarına erişebilirsin
                query = query.Where(s => s.Name.Contains(searchText));
            }

            // Distinct
            var distinctQuery = query.Distinct();

            var totalCount = await distinctQuery.CountAsync(token);

            var suppliers = await distinctQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
                .ToListAsync(token);

            return new PaginatedResponse<SupplierDto>(suppliers, totalCount, page, size);
        }
    }
}
