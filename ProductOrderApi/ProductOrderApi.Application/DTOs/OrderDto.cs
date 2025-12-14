using ProductOrderApi.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}