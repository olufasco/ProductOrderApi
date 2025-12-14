namespace ProductOrderApi.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; } = null!;
        public string? PictureUrl { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Concurrency token
        public byte[] RowVersion { get; set; } = null!;
    }
}
