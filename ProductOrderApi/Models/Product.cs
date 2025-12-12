namespace ProductOrderApi.Models
{
    public class Product : BaseEntity
    {
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public List <Picture> Pictures { get; set; } = new();
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Pending;
    }

}
