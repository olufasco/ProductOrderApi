namespace ProductOrderApi.Application.DTOs
{
    public class AddToCartDto
    {
        public string SKU { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
