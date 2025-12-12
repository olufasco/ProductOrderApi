
namespace ProductOrderApi.Models
{
    public class Order : BaseEntity
    {
        public decimal TotalAmount { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new();
        public Guid UserId { get; internal set; }
        public string CustomerName { get; set; } = default!;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}