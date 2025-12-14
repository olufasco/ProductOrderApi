namespace ProductOrderApi.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
