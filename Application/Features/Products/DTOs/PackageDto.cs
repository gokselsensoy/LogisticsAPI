namespace Application.Features.Products.DTOs
{
    public class PackageDto
    {
        public Guid PackageId { get; set; }
        public string Name { get; set; } // "6'lı Koli"
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
}
