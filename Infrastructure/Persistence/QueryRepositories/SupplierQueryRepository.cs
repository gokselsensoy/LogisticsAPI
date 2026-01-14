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
            // 1. User Location (Point)
            var userLocation = await _context.Set<CustomerAddress>()
                .AsNoTracking()
                .Where(a => a.Id == addressId)
                .Select(a => a.Address.Location)
                .FirstOrDefaultAsync(token);

            if (userLocation == null) return new PaginatedResponse<SupplierDto>(new List<SupplierDto>(), 0, page, size);

            // 2. Query (PostGIS Optimizasyonu)
            var sortedSuppliersQuery = _context.Set<Terminal>()
                .AsNoTracking()
                .Where(t => !t.IsDeleted && t.ServiceRadiusKm.HasValue)
                // DİKKAT:
                // Eğer ColumnType "geography" ise IsWithinDistance METRE çalışır.
                // Bu fonksiyon PostGIS'in ST_DWithin fonksiyonuna çevrilir ve INDEX kullanır (Çok hızlıdır).
                .Where(t => t.Address.Location.IsWithinDistance(userLocation, t.ServiceRadiusKm.Value * 1000))

                .Where(t => t.Department.Company is Supplier) // Sadece Supplierlar
                .Select(t => new
                {
                    SupplierId = t.Department.Company.Id,
                    SupplierName = t.Department.Company.Name,
                    // Burada Distance hesabı yapıyoruz (Sıralama için)
                    // ColumnType geography olduğu için sonuç METRE döner.
                    Distance = t.Address.Location.Distance(userLocation)
                })
                .Where(x => string.IsNullOrEmpty(searchText) || x.SupplierName.Contains(searchText))

                // 3. GroupBy ve Sıralama (Aynı)
                .GroupBy(x => new { x.SupplierId, x.SupplierName })
                .Select(g => new
                {
                    g.Key.SupplierId,
                    g.Key.SupplierName,
                    MinDistance = g.Min(x => x.Distance)
                })
                .OrderBy(x => x.MinDistance); // En yakına göre sırala

            // 4. Pagination (Aynı)
            var totalCount = await sortedSuppliersQuery.CountAsync(token);

            var pagedResult = await sortedSuppliersQuery
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(token);

            // 5. Mapping
            var dtos = pagedResult.Select(x => new SupplierDto
            {
                Id = x.SupplierId,
                Name = x.SupplierName
                // LogoUrl vs..
            }).ToList();

            return new PaginatedResponse<SupplierDto>(dtos, totalCount, page, size);
        }
    }
}
