namespace ProductOrderApi.Models
{
    public class OrderLine : BaseEntity
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; internal set; }
    }
}
