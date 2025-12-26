using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Inventories.Commands.AddStock
{
    public class AddStockCommandHandler : IRequestHandler<AddStockCommand, Unit>
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IInventoryTransactionRepository _transactionRepo; // Transaction kaydı için
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        // Package kontrolü için IProductRepository gerekebilir (PackageId geçerli mi diye)
        // Ama aggregate root olmadığı için Product üzerinden gitmek veya basit bir check yeterli.

        public AddStockCommandHandler(
            IInventoryRepository inventoryRepo,
            IInventoryTransactionRepository transactionRepo,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _inventoryRepo = inventoryRepo;
            _transactionRepo = transactionRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddStockCommand request, CancellationToken token)
        {
            // 1. Envanteri Getir (Stoklarıyla beraber)
            var inventory = await _inventoryRepo.GetByIdWithStocksAsync(request.InventoryId, token);
            if (inventory == null) throw new Exception("Lokasyon bulunamadı.");

            // 3. Worker Bilgisi
            // İşlemi yapan kişi (Worker ID)
            if (!_currentUser.ProfileId.HasValue)
                throw new UnauthorizedAccessException("Worker profili bulunamadı.");
            var workerId = _currentUser.ProfileId.Value;

            // 4. Domain Logic: Stok Ekle
            // AddStock metodu bize oluşan Transaction kaydını geri döner.
            var transaction = inventory.AddStock(
                request.PackageId,
                request.Quantity,
                request.OwnerId,
                request.State,
                workerId,
                request.Note
            );

            // 5. Değişiklikleri Kaydet
            // Inventory üzerinde yapılan değişiklik (Stocks listesine ekleme) EF Core tarafından algılanır.
            _inventoryRepo.Update(inventory);

            // Transaction kaydını da ekle (Loglama)
            _transactionRepo.Add(transaction);

            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
