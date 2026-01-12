using Application.Abstractions.Messaging;
using Application.Shared.Models;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Features.Baskets.Commands.Checkout
{
    public class CheckoutCommand : ICommand<Guid>
    {
        public Guid BasketId { get; set; }
        public Guid DeliveryAddressId { get; set; }

        public AddressDto? ManualAddress { get; set; }

        public PaymentChannel PaymentInfo { get; set; }
    }
}
