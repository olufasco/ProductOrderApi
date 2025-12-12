namespace ProductOrderApi.DTOs
{
    public class CartDtos
    {
        public record CartItemDto(
            Guid Id,
            Guid ProductId,
            string Sku,
            string Name,
            int Quantity,
            decimal UnitPrice,
            decimal LineTotal
        );

        public record CartDto(
            Guid Id,
            Guid UserId,
            decimal TotalAmount,
            List<CartItemDto> Items
        );

        public record AddToCartDto(
            Guid ProductId,
            string Sku,
            int Quantity
        );

        public record UpdateCartItemDto(
            Guid CartItemId,
            int Quantity
        );
    }
}