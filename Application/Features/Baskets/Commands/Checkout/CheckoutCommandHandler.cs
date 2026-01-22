using Application.Abstractions.Services;
using Application.Shared.Extensions;
using Domain.Entities.Orders;
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
        private readonly IOrderGroupRepository _orderGroupRepo;
        private readonly IProductRepository _productRepo; // Paket detayları için
        private readonly ICustomerRepository _customerRepo;
        private readonly IInventoryRepository _inventoryRepo; // Domain Service (Stok kontrolü)
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutCommandHandler(IBasketRepository basketRepo,
            IOrderGroupRepository orderGroupRepo,
            IProductRepository productRepo,
            ICustomerRepository customerRepo,
            IInventoryRepository inventoryRepo,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _basketRepo = basketRepo;
            _orderGroupRepo = orderGroupRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _inventoryRepo = inventoryRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CheckoutCommand request, CancellationToken token)
        {
            // 1. Sepeti Getir
            var basket = await _basketRepo.GetByIdWithItemsAsync(request.BasketId, token);
            if (basket == null || !basket.Items.Any())
                throw new DomainException("Sepet boş veya bulunamadı.");

            // 2. Müşteri Bilgilerini Getir
            var customer = await _customerRepo.GetByIdWithAddressesAsync(_currentUser.ProfileId.Value, token);
            if (customer == null) throw new Exception("Müşteri profili bulunamadı.");

            // 3. Adres Belirleme
            Address deliveryAddress;
            Guid? customerAddressId = null;

            var addr = customer.Addresses.FirstOrDefault(a => a.Id == request.DeliveryAddressId);
            if (addr == null) throw new DomainException("Seçilen teslimat adresi bulunamadı.");

            deliveryAddress = addr.Address; // Snapshot
            customerAddressId = addr.Id;

            // 4. Contact Info Oluştur
            var contactInfo = new ContactInfo(customer.Name, customer.Email, customer.PhoneNumber);

            // 5. Payment Context Oluştur
            var paymentContext = new PaymentContext(request.PaymentInfo, PaymentStatus.Pending);

            // 6. OrderGroup (Master) Oluştur
            var orderNumber = GenerateOrderNumber(); // Helper
            var orderGroup = new OrderGroup(customer.Id, orderNumber, paymentContext);

            // 7. Sepeti Tedarikçiye Göre Grupla
            var supplierGroups = basket.Items.GroupBy(i => i.SupplierId);

            foreach (var group in supplierGroups)
            {
                var supplierId = group.Key;

                // 7a. Alt Sipariş (Order) Oluştur
                var order = new Order(
                    orderGroup.Id,
                    OrderOrigin.InternalSystem,
                    supplierId,
                    customer.Id, // Denormalized
                    deliveryAddress,
                    contactInfo,
                    customerAddressId
                );

                // 7b. Itemları Ekle
                foreach (var item in group)
                {
                    var package = await _productRepo.GetPackageByIdAsync(item.PackageId, token);
                    if (package == null) throw new DomainException($"Paket bulunamadı (ID: {item.PackageId})");

                    double volumeM3 = (package.Dimensions.Width * package.Dimensions.Height * package.Dimensions.Length) / 1_000_000;

                    var cargoSpec = new CargoSpec(
                        desc: package.Name,
                        weight: package.Dimensions.Weight,
                        volume: volumeM3
                    );

                    // Item Ekle
                    order.AddItem(
                        package.Id,
                        package.Name,
                        cargoSpec,
                        item.Quantity,
                        package.Price
                    );

                    // 7c. Stok Rezerve Et
                    // REZERVE LOGIC (Aynı kalıyor)
                    var inventory = await _inventoryRepo.GetFirstWithStockAsync(package.Id, supplierId, item.Quantity, InventoryState.Available, token);
                    if (inventory == null)
                        throw new DomainException($"'{package.Name}' ürünü için depoda yeterli stok bulunamadı.");

                    inventory.ReserveStock(package.Id, item.Quantity, supplierId);
                    _inventoryRepo.Update(inventory);
                }

                // 7d. Order'ı Gruba Ekle
                order.SetCommission(0); // Şimdilik 0
                orderGroup.AddOrder(order);
            }

            // 8. Ödeme Durumu ve Kayıt
            if (paymentContext.Channel == PaymentChannel.OnPlatform)
            {
                orderGroup.MarkAsPaid(); // Event fırlatır -> Handler çalışır -> Shipments oluşur
            }

            // OrderGroup kaydedilince cascade ile Orders ve Items kaydedilir.
            _orderGroupRepo.Add(orderGroup);
            _basketRepo.Delete(basket);

            await _unitOfWork.SaveChangesAsync(token);

            return orderGroup.Id;
        }

        private string GenerateOrderNumber()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(1000, 9999);
        }
    }
}

