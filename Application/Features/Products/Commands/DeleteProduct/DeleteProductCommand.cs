using Application.Abstractions.Messaging;
using Domain.Enums;
using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : ICommand<Unit>
    {
        public Guid Id { get; set; }
    }
}
