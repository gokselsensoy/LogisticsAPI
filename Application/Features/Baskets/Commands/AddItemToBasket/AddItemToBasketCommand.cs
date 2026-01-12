using Application.Abstractions.Messaging;

namespace Application.Features.Baskets.Commands.AddItemToBasket
{
    public class AddItemToBasketCommand : ICommand<Guid>
    {
        public Guid PackageId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
