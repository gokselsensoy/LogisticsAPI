using Application.Abstractions.Services;
using Application.Shared.Extensions;
using Domain.Entities.Order;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Baskets.Commands.Checkout
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Guid>
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo; // Paket detayları için
        private readonly ICustomerRepository _customerRepo;
        private readonly IInventoryRepository _inventoryRepo; // Domain Service (Stok kontrolü)
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutCommandHandler(IBasketRepository basketRepo,
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            ICustomerRepository customerRepo,
            IInventoryRepository inventoryRepo,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _basketRepo = basketRepo;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _inventoryRepo = inventoryRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CheckoutCommand request, CancellationToken token)
        {
            // 1. Sepeti Getir
            // HATA ÇÖZÜMÜ: GetByIdWithItemsAsync metodunu BasketRepository'de tanımlamıştık.
            var basket = await _basketRepo.GetByIdWithItemsAsync(request.BasketId, token);
            if (basket == null || !basket.Items.Any())
                throw new DomainException("Sepet boş veya bulunamadı.");

            // 2. Müşteri Bilgilerini Getir (ContactInfo için Email/Phone lazım)
            // HATA ÇÖZÜMÜ: _currentUser.Email yoktu, DB'den Customer çekiyoruz.
            // Not: Eğer CorporateResponsible ise bağlı olduğu Customer'ı çekmek gerekebilir. 
            // Burada basitlik adına giriş yapan ID'nin Customer tablosunda karşılığı olduğunu varsayıyoruz (Individual veya Corporate).
            var customer = await _customerRepo.GetByIdWithAddressesAsync(_currentUser.ProfileId.Value, token);
            if (customer == null) throw new Exception("Müşteri profili bulunamadı.");

            // 3. Adres Belirleme
            Address deliveryAddress;
            Guid? customerAddressId = null;

            if (request.ManualAddress != null)
            {
                // HATA ÇÖZÜMÜ: Extension metodunu yukarıda yazdık.
                deliveryAddress = request.ManualAddress.ToValueObject();
            }
            else
            {
                var addr = customer.Addresses.FirstOrDefault(a => a.Id == request.DeliveryAddressId);
                if (addr == null) throw new DomainException("Seçilen teslimat adresi bulunamadı.");

                deliveryAddress = addr.Address; // Snapshot
                customerAddressId = addr.Id;
            }

            // 4. Contact Info Oluştur
            // Customer entity'sinden gelen verilerle oluşturuyoruz.
            // ContactInfo constructor sırasına dikkat: (Name, Email, Phone) varsayıyorum.
            var contactInfo = new ContactInfo(customer.Name, customer.Email, customer.PhoneNumber);

            // 5. Payment Context Oluştur
            // HATA ÇÖZÜMÜ: PaymentContext constructor'ı PaymentChannel alıyor.
            var paymentContext = new PaymentContext(request.PaymentInfo);

            // 6. Order Oluştur (Draft)
            // Sepetteki ilk ürünün tedarikçisini siparişin tedarikçisi yapıyoruz (Multi-supplier sepet yok varsayımı)
            var firstItem = basket.Items.First();

            var order = new Order(
                OrderOrigin.InternalSystem,
                firstItem.SupplierId,
                paymentContext,
                deliveryAddress,
                contactInfo,
                customer.Id,
                customerAddressId
            );

            // 7. Sepet Kalemlerini Siparişe Dönüştür ve STOK REZERVE ET
            foreach (var item in basket.Items)
            {
                var package = await _productRepo.GetPackageByIdAsync(item.PackageId, token);
                if (package == null) throw new DomainException($"Paket bulunamadı (ID: {item.PackageId})");

                double volumeM3 = (package.Dimensions.Width * package.Dimensions.Height * package.Dimensions.Length) / 1_000_000;

                // CargoSpec oluştur
                var cargoSpec = new CargoSpec(
                    desc: package.Name, // veya package.Description
                    weight: package.Dimensions.Weight,
                    volume: volumeM3
                );

                // A. Sipariş Kalemi Ekle
                order.AddItem(
                    package.Id,
                    package.Name,
                    cargoSpec, // <-- Artık hata vermez
                    item.Quantity,
                    package.Price
                );

                // B. Stok Rezerve Et
                // HATA ÇÖZÜMÜ: IInventoryRepository'e eklediğimiz metodu kullanıyoruz
                var supplierId = package.Product.SupplierId;
                var inventory = await _inventoryRepo.GetFirstWithStockAsync(package.Id, supplierId, item.Quantity, InventoryState.Available, token);

                if (inventory == null)
                    throw new DomainException($"'{package.Name}' ürünü için depoda yeterli stok bulunamadı.");

                // Domain Metodunu Çağır (Available -> Reserved)
                inventory.ReserveStock(package.Id, item.Quantity, supplierId);

                _inventoryRepo.Update(inventory);
            }

            // 8. Siparişi Tamamla
            order.ConfirmOrder();

            // Eğer kredi kartı ise hemen ödendi işaretle (Mock)
            if (paymentContext.Channel == PaymentChannel.OnPlatform)
            {
                order.MarkAsPaid();
            }

            _orderRepo.Add(order);
            _basketRepo.Delete(basket);

            await _unitOfWork.SaveChangesAsync(token);

            return order.Id;
        }
    }
}

