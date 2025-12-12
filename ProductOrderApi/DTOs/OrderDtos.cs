using ProductOrderApi.Models;

namespace ProductOrderApi.DTOs
{
    public class OrderDtos
    {
        public record OrderLineDto(
            Guid Id,
            Guid ProductId,
            string Sku,
            string Name,
            int Quantity,
            decimal UnitPrice,
            decimal LineTotal
        );

        public record OrderDto(
            Guid Id,
            Guid UserId,
            string CustomerName,
            decimal TotalAmount,
            List<OrderLineDto> OrderLines
        );

        public record CheckoutDto(
            Guid CartId,
            string CustomerName
        );
        public record UpdateOrderDto(
            string? CustomerName, 
            OrderStatus? Status
        );

        public record PlaceOrderRequest(
            Guid CartId, 
            string CustomerName,
            List<OrderLineDto> OrderLines
        );
    }

}