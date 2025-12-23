using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.Features.Products.Commands.AddPackage
{
    public class AddPackageCommand : ICommand<Guid>
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } // "6-Pack"
        public PackageType Type { get; set; } // Box, Pallet
        public decimal ConversionFactor { get; set; } // 6, 24, 1200
        public string Barcode { get; set; }

        // Fiyat
        public decimal PriceAmount { get; set; }
        public string Currency { get; set; } = "TRY";

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
