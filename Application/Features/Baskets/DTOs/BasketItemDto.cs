namespace Application.Features.Baskets.DTOs
{
    public class BasketItemDto
    {
        public Guid PackageId { get; set; }
        public string PackageName { get; set; } // "6-Pack Kola"
        public string SupplierName { get; set; } // "Coca Cola Dağıtım A.Ş."

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal TotalLinePrice { get; set; } // Quantity * UnitPrice
        public string Currency { get; set; }

        // Opsiyonel: Resim URL vs.
        // public string ImageUrl { get; set; }
    }
}
