namespace ProductOrderApi.API.ProductOrderApi.Application.DTOs
{
    public class CreateOrderItemDto
    {
        public string SKU { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
