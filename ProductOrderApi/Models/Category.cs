namespace ProductOrderApi.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Slug { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}