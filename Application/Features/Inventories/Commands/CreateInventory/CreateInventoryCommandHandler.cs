using Application.Abstractions.Services;
using Domain.Entities.Inventories;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Inventories.Commands.CreateInventory
{
    public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, Guid>
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInventoryCommandHandler(IInventoryRepository inventoryRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _inventoryRepo = inventoryRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateInventoryCommand request, CancellationToken token)
        {

            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Depo/Terminal bilgisi bulunamadı.");

            // 2. Sanal (Virtual) Envanter Kontrolü
            Inventory inventory;
            if (request.IsVirtual)
            {
                inventory = Inventory.CreateVirtual(request.TerminalId);
            }
            else
            {
                inventory = new Inventory(request.TerminalId, request.Area, request.Corridor, request.Place, request.Shelf, false);
            }

            // 3. Lokasyon Kodu Unique mi?
            if (await _inventoryRepo.IsLocationExistsAsync(request.TerminalId, inventory.LocationCode, token))
                throw new Exception($"Bu lokasyon kodu ({inventory.LocationCode}) zaten mevcut.");

            // 4. Kaydet
            _inventoryRepo.Add(inventory);
            await _unitOfWork.SaveChangesAsync(token);

            return inventory.Id;
        }
    }
}
