using Application.Abstractions.Messaging;

namespace Application.Features.Inventories.Commands.CreateInventory
{
    public class CreateInventoryCommand : ICommand<Guid>
    {
        public Guid TerminalId { get; set; }
        public string Area { get; set; }
        public string? Corridor { get; set; }
        public string? Place { get; set; }
        public string? Shelf { get; set; }
        public bool IsVirtual { get; set; }
    }
}
