namespace Application.Features.Baskets.DTOs
{
    public class BasketDto
    {
        public Guid Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "TRY";
        public List<BasketItemDto> Items { get; set; } = new();
    }
}
