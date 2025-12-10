using Domain.Entities.Inventory;
using Domain.Events.TerminalEvents.Domain.Events;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Terminals.EventHandlers
{
    public class CreateVirtualInventoryWhenTerminalCreated : INotificationHandler<TerminalCreatedEvent>
    {
        private readonly IInventoryRepository _inventoryRepo;
        // Not: UnitOfWork inject etmiyoruz çünkü bu işlem, Terminal oluşturma işlemiyle
        // aynı Transaction içinde çalışacak (EF Core davranışı).
        // Terminal save edildiği an event fırlatılır, bu kod çalışır ve tekrar save gerektirmez
        // EĞER "DomainEvents" yapısını SaveChangesAsync içinde "Önce Kaydet Sonra Fırlat" şeklinde kurduysak
        // O zaman burada yeni bir SaveChanges çağırmamız gerekebilir.

        // Bizim kurgumuzda "SaveChangesAsync" içinde önce DB'ye yazıp sonra Event fırlatıyorduk.
        // Bu yüzden burada _inventoryRepo.Add yapıp ayrıca SaveChanges çağırmalıyız.

        private readonly IUnitOfWork _unitOfWork;

        public CreateVirtualInventoryWhenTerminalCreated(
            IInventoryRepository inventoryRepo,
            IUnitOfWork unitOfWork)
        {
            _inventoryRepo = inventoryRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(TerminalCreatedEvent notification, CancellationToken cancellationToken)
        {
            // 1. Sanal Envanteri Oluştur (Factory Method ile)
            var virtualInventory = Inventory.CreateVirtual(notification.TerminalId);

            // 2. Repoya Ekle
            _inventoryRepo.Add(virtualInventory);

            // 3. Kaydet
            // Not: Bu handler, Terminal kaydedildikten SONRA çalıştığı için (Interceptor yapımıza göre)
            // burada tekrar SaveChangesAsync çağırmalıyız.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
