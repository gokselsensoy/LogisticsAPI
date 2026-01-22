using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Events.OrderEvents;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Baskets.EventHandlers
{
    public class OrderGroupPaidEventHandler : INotificationHandler<OrderGroupPaidEvent>
    {
        private readonly IOrderGroupRepository _orderGroupRepo;
        private readonly IShipmentRepository _shipmentRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly ITerminalRepository _terminalRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderGroupPaidEventHandler(
            IOrderGroupRepository orderGroupRepo,
            IShipmentRepository shipmentRepo,
            IInventoryRepository inventoryRepo,
            ITerminalRepository terminalRepo,
            IUnitOfWork unitOfWork)
        {
            _orderGroupRepo = orderGroupRepo;
            _shipmentRepo = shipmentRepo;
            _inventoryRepo = inventoryRepo;
            _terminalRepo = terminalRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderGroupPaidEvent notification, CancellationToken token)
        {
            // 1. OrderGroup'u ve Alt Siparişleri Getir
            var orderGroup = await _orderGroupRepo.GetByIdWithOrdersAsync(notification.OrderGroupId, token);
            if (orderGroup == null) return; // veya throw

            // 2. Her bir alt sipariş için işlem yap
            foreach (var order in orderGroup.Orders)
            {
                // A. Siparişi Ödendi Olarak İşaretle (Domain Logic)
                if (order.PaymentStatus != PaymentStatus.Paid)
                {
                    order.MarkAsPaid();
                }

                // B. Shipment Oluşturma Mantığı
                if (!order.Items.Any()) continue;

                var firstItem = order.Items.First();

                // Bu paket, bu supplier adına, REZERVE (Reserved) statüsünde hangi Inventory'de var?
                var sourceInventory = await _inventoryRepo.GetFirstWithStockAsync(
                    firstItem.PackageId,
                    order.SupplierId,
                    firstItem.Quantity,
                    InventoryState.Reserved,
                    token
                );

                if (sourceInventory == null)
                {
                    // Log error but maybe don't stop strictly? 
                    // throw new DomainException($"Sipariş #{order.Id} için rezerve stok (Inventory) bulunamadı!");
                    // Şimdilik throw
                    throw new DomainException($"Sipariş #{order.Id} için rezerve stok (Inventory) bulunamadı!");
                }

                // Inventory -> Terminal -> Adres
                var terminal = await _terminalRepo.GetByIdAsync(sourceInventory.TerminalId, token);
                Address pickupAddress = terminal.Address;

                // C. Shipment Oluştur
                var shipment = new Shipment(
                    ShipmentType.Delivery,
                    ShipmentSource.Order,
                    pickupAddress,
                    order.DeliveryAddressSnapshot, // Order'dan snapshot
                    order.Id
                );

                // D. Itemları Ekle
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

            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
