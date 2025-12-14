namespace ProductOrderApi.Application.DTOs
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }

        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
