using Domain.Entities.Order;
using Domain.Enums;
using Domain.Events.OrderEvents;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Baskets.EventHandlers
{
    public class OrderPaidEventHandler : INotificationHandler<OrderPaidEvent>
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IShipmentRepository _shipmentRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly ITerminalRepository _terminalRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderPaidEventHandler(
            IOrderRepository orderRepo,
            IShipmentRepository shipmentRepo,
            IInventoryRepository inventoryRepo,
            ITerminalRepository terminalRepo,
            IUnitOfWork unitOfWork)
        {
            _orderRepo = orderRepo;
            _shipmentRepo = shipmentRepo;
            _inventoryRepo = inventoryRepo;
            _terminalRepo = terminalRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderPaidEvent notification, CancellationToken token)
        {
            var order = notification.Order;

            // 1. Pickup Adresini (Terminal Adresi) Bul
            // Senaryo: Sipariş tek bir yerden çıkacak varsayıyoruz (Split shipment yok).
            // İlk ürünün rezerve edildiği Inventory'yi buluyoruz.

            var firstItem = order.Items.First();

            // TODO: İleride müşteri adresine en yakın Terminali seçen algoritma buraya gelecek.
            // Şimdilik: Bu paket, bu supplier adına, REZERVE (Reserved) statüsünde hangi Inventory'de var?
            // Checkout sırasında rezerve etmiştik.
            var sourceInventory = await _inventoryRepo.GetFirstWithStockAsync(
                firstItem.PackageId.Value,
                order.SupplierId,
                firstItem.Quantity,
                InventoryState.Reserved, // DİKKAT: Artık Available değil Reserved arıyoruz!
                token
            );

            if (sourceInventory == null)
            {
                // Kritik Hata: Sipariş ödenmiş ama rezerve stok bulunamıyor!
                // Admin'e alert atılmalı veya manuel incelemeye düşmeli.
                throw new DomainException($"Sipariş #{order.Id} için rezerve stok (Inventory) bulunamadı!");
            }

            // Inventory -> Terminal -> Adres
            var terminal = await _terminalRepo.GetByIdAsync(sourceInventory.TerminalId, token);
            // Not: Terminal entity'sinde Address ValueObject olduğunu varsayıyorum.
            Address pickupAddress = terminal.Address;

            // 2. Shipment Oluştur
            var shipment = new Shipment(
                ShipmentType.Delivery,
                ShipmentSource.Order,
                pickupAddress,
                order.DeliveryAddressSnapshot,
                order.Id
            );

            // 3. Itemları Ekle
            foreach (var item in order.Items)
            {
                shipment.AddShipmentItem(
                    item.Id,
                    item.PackageId,
                    item.SpecSnapshot,
                    item.Quantity
                );
            }

            _shipmentRepo.Add(shipment);
        }
    }
}
