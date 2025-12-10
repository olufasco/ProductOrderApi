namespace ProductOrderApi.DTOs
{
    public class OrderDtos
    {
        public record OrderLineRequest(Guid ProductId, int Quantity);
        public record PlaceOrderRequest(List<OrderLineRequest> Lines, string? IdempotencyKey);
        public record OrderDto(Guid Id, DateTimeOffset CreatedAt, decimal TotalAmount, List<OrderLineDto> Lines);
        public record OrderLineDto(Guid ProductId, int Quantity, decimal UnitPrice, decimal LineTotal);
    }
}
