using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Baskets.Commands.RemoveItemFromBasket
{
    public class RemoveItemFromBasketCommand : ICommand<Unit>
    {
        public Guid PackageId { get; set; }
        // public int Quantity { get; set; } // Opsiyonel: Miktar düşürmek için eklenebilir
    }
}
