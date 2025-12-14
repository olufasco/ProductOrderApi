namespace ProductOrderApi.Application.DTOs
{
    public class ProductDto
    {
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}