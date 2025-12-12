namespace ProductOrderApi.Models
{
    public class OrderLine : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}