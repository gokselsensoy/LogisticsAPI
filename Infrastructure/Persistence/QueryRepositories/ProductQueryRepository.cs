using Application.Abstractions.EntityRepositories;
using Application.Features.Products.DTOs;
using Application.Shared.Pagination;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public async Task<PaginatedResponse<ProductListingDto>> SearchProductsAsync(
            double lat, double lon, string? searchText, ProductCategory? category, Guid? supplierId, int page, int size, CancellationToken token)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var userLocation = geometryFactory.CreatePoint(new Coordinate(lon, lat));

            // 1. Kapsama Alanındaki Inventory ID'lerini bul (Subquery olarak kullanılacak)
            var nearbyInventoryIds = _context.Set<Terminal>()
                .AsNoTracking()
                .Where(t => !t.IsDeleted && t.Address.Location.Distance(userLocation) <= (t.ServiceRadiusKm * 1000))
                .SelectMany(t => t.Inventories)
                .Select(i => i.Id);

            // 2. Ürün Sorgusu
            // Stock -> Package -> Product -> Supplier zinciri
            // Burada ProjectTo kullanabilmek için Query'yi "Product" tablosu üzerinden kurmak
            // ve filtreleri "Any" ile alt tablolara uygulamak en temizidir.

            var query = _context.Set<Product>()
                .AsNoTracking()
                .Where(p => !p.IsDeleted);

            // -- Filtreler --

            // A. Konum Filtresi (En karmaşığı):
            // "Ürünün paketlerinden herhangi birinin stoğu, yakındaki inventory'lerde var mı?"
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

            // C. Kategori
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

            // 3. ProjectTo ile DTO Dönüşümü
            var products = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ProjectTo<ProductListingDto>(_mapper.ConfigurationProvider) // <-- AUTOMAPPER
                .ToListAsync(token);

            // Not: Stok var mı yok mu bilgisi için ProductListingDto içine bir boolean eklemiştik.
            // Bunu AutoMapper profilinde .ForMember ile karmaşık bir LINQ ifadesi yazarak mapleyebilirsin
            // veya burada products geldikten sonra doldurabilirsin.
            // Ancak ProjectTo ile SQL'e gömmek en iyisidir. 
            // Profilde: .ForMember(d => d.IsInStock, o => o.MapFrom(s => s.Packages.Any(pk => pk.Stocks.Any(st => st.Quantity > 0))))

            return new PaginatedResponse<ProductListingDto>(products, totalCount, page, size);
        }
    }
}
