using Application.Abstractions.Messaging;
using Application.Features.Products.Commands.DeleteProduct;
using MediatR;

namespace Application.Features.Products.Commands.DeletePackage
{
    public class DeletePackageCommand : ICommand<Unit>
    {
        public Guid ProductId { get; set; }
        public Guid PackageId { get; set; }
    }
}
