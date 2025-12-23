using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Products.Commands.UpdatePackage
{
    public class UpdatePackageCommand : ICommand<Unit>
    {
        public Guid ProductId { get; set; }
        public Guid PackageId { get; set; }

        public string Name { get; set; }

        // Fiyat
        public decimal PriceAmount { get; set; }
        public string Currency { get; set; } = "DK";

        // Boyutlar
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }

        // Depozito
        public bool IsReturnable { get; set; }
        public decimal? DepositAmount { get; set; }
    }
}
