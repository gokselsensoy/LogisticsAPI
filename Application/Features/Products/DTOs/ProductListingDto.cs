namespace Application.Features.Products.DTOs
{
    public class ProductListingDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Category { get; set; }

        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }

        // Paket Listesi (Fiyatlarıyla)
        public List<PackageDto> Packages { get; set; }

        public bool IsInStock { get; set; } // Stok var mı?
    }
}
