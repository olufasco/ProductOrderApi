namespace ProductOrderApi.Models
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }  
        public List<CartItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; } 
    }
}