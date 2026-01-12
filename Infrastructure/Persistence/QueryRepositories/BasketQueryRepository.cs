using Application.Abstractions.EntityRepositories;
using Application.Features.Baskets.DTOs;
using Domain.Entities.Inventories;
using Domain.Entities.Orders;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryRepositories
{
    public class BasketQueryRepository : IBasketQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public BasketQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BasketDto?> GetBasketDetailsByCustomerIdAsync(Guid customerId, CancellationToken token)
        {
            // 1. Önce Kullanıcının Sepetini Bul
            var basketId = await _context.Set<Basket>()
                .AsNoTracking()
                .Where(b => b.CustomerId == customerId)
                .Select(b => b.Id)
                .FirstOrDefaultAsync(token);

            if (basketId == Guid.Empty) return null; // Sepet yok

            // 2. Sepet Kalemlerini ve Ürün Detaylarını Çek (JOIN İşlemi)
            // BasketItem tablosunu Package ve Product tablolarıyla birleştiriyoruz.

            var itemsQuery = from bi in _context.Set<BasketItem>().AsNoTracking()
                             join pkg in _context.Set<Package>().AsNoTracking()
                                on bi.PackageId equals pkg.Id
                             join prod in _context.Set<Product>().AsNoTracking()
                                on pkg.ProductId equals prod.Id
                             // Eğer Supplier adını da istiyorsak Customer/Company tablosuna da join atabiliriz
                             // join supp in _context.Set<Customer>().AsNoTracking() on prod.SupplierId equals supp.Id

                             where bi.BasketId == basketId
                             select new BasketItemDto
                             {
                                 PackageId = pkg.Id,
                                 PackageName = pkg.Name, // Paket Adı
                                 SupplierName = prod.Name, // Şimdilik Ürün Adı veya Supplier ID (Basitlik için)

                                 Quantity = bi.Quantity,

                                 // Value Object (Money) içindeki Amount'u alıyoruz
                                 UnitPrice = pkg.Price.Amount,
                                 Currency = pkg.Price.Currency,

                                 TotalLinePrice = pkg.Price.Amount * bi.Quantity
                             };

            var items = await itemsQuery.ToListAsync(token);

            if (!items.Any()) return new BasketDto { Id = basketId }; // Boş sepet

            // 3. Toplam Tutarı Hesapla ve DTO'yu Dön
            var basketDto = new BasketDto
            {
                Id = basketId,
                Items = items,
                Currency = items.First().Currency, // Genelde hepsi aynıdır varsayıyoruz
                TotalPrice = items.Sum(x => x.TotalLinePrice)
            };

            return basketDto;
        }
    }
}
