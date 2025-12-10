namespace ProductOrderApi.Models
{
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public List<OrderLine> Lines { get; set; } = new();
    }

}
