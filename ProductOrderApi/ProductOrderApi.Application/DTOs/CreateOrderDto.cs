namespace ProductOrderApi.API.ProductOrderApi.Application.DTOs
{
    public class CreateOrderDto
    {
        public ICollection<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}