using Application.Abstractions.Messaging;
using Domain.Enums;
using MediatR;

namespace Application.Features.Inventories.Commands.AddStock
{
    public class AddStockCommand : ICommand<Unit>
    {
        public Guid InventoryId { get; set; }
        public Guid PackageId { get; set; }
        public int Quantity { get; set; }
        public Guid OwnerId { get; set; }
        public InventoryState State { get; set; } = InventoryState.Available;
        public string? Note { get; set; }
    }
}
