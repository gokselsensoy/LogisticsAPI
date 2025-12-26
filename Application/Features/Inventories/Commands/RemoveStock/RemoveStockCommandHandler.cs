using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Inventories.Commands.RemoveStock
{
    public class RemoveStockCommandHandler : IRequestHandler<RemoveStockCommand, Unit>
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IInventoryTransactionRepository _transactionRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveStockCommandHandler(
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

        public async Task<Unit> Handle(RemoveStockCommand request, CancellationToken token)
        {
            var inventory = await _inventoryRepo.GetByIdWithStocksAsync(request.InventoryId, token);
            if (inventory == null) throw new Exception("Lokasyon bulunamadı.");

            var workerId = _currentUser.ProfileId.Value;

            // Domain Logic: Stok Çıkar
            // Yetersiz stok varsa Exception fırlatır (Domain içinde kontrol ediliyor)
            var transaction = inventory.RemoveStock(
                request.PackageId,
                request.Quantity,
                request.OwnerId,
                request.State,
                workerId,
                request.Note
            );

            _inventoryRepo.Update(inventory);
            _transactionRepo.Add(transaction);

            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
