using Application.Abstractions.EntityRepositories;
using Application.Features.Products.DTOs;
using Application.Shared.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities.Customers;
using Domain.Entities.Departments;
using Domain.Entities.Inventories;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Infrastructure.Persistence.QueryRepositories
{
    public class ProductQueryRepository : IProductQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductQueryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<ProductListingDto>> SearchProductsByAddressAsync(
    Guid addressId, string? searchText, ProductCategory? category, Guid? supplierId, int page, int size, CancellationToken token)
        {
            // 1. ADIM: AddressId'den Kullanıcının Koordinatını (Point) Bul
            var userLocation = await _context.Set<CustomerAddress>()
                .AsNoTracking()
                .Where(a => a.Id == addressId)
                .Select(a => a.Address.Location) // Direkt Point nesnesi
                .FirstOrDefaultAsync(token);

            // Adres bulunamazsa boş dön
            if (userLocation == null)
            {
                return new PaginatedResponse<ProductListingDto>(new List<ProductListingDto>(), 0, page, size);
            }

            // 2. ADIM: Yakındaki Stokların Olduğu Inventory ID'lerini Bul
            // (Veritabanındaki Point ile UserLocation Point karşılaştırılıyor)
            var nearbyInventoryIds = _context.Set<Terminal>()
                .AsNoTracking()
                .Where(t => !t.IsDeleted && t.ServiceRadiusKm.HasValue)
                .Where(t => t.Address.Location.Distance(userLocation) <= (t.ServiceRadiusKm * 1000))
                .SelectMany(t => t.Inventories)
                .Select(i => i.Id);

            // 3. ADIM: Ürün Sorgusunu Oluştur
            var query = _context.Set<Product>()
                .AsNoTracking()
                .Where(p => !p.IsDeleted);

            // -- Filtreler --

            // A. Stok/Konum Filtresi
            query = query.Where(p => p.Packages.Any(pkg =>
                pkg.Stocks.Any(s =>
                    nearbyInventoryIds.Contains(s.InventoryId) &&
                    s.State == InventoryState.Available &&
                    s.Quantity > 0
                )));

            // B. Text Arama
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(p => p.Name.Contains(searchText) || p.Supplier.Name.Contains(searchText));
            }

            // C. Kategori (Enum)
            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            // D. Supplier ID
            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            var totalCount = await query.CountAsync(token);

            // 4. ADIM: Sonuçları Getir ve Map'le
            var products = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ProjectTo<ProductListingDto>(_mapper.ConfigurationProvider)
                .ToListAsync(token);

            return new PaginatedResponse<ProductListingDto>(products, totalCount, page, size);
        }
    }
}
