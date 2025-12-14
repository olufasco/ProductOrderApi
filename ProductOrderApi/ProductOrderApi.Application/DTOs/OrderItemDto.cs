using ProductOrderApi.Application.DTOs;
public class OrderItemDto
{
    public string SKU { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}