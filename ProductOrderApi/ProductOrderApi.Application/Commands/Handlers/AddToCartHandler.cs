using ProductOrderApi.Domain.Entities;
namespace ProductOrderApi.Application.Commands.Handlers
{
    public class AddToCartHandler
    {
        private readonly IUnitOfWork _uow;
        public AddToCartHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<string>> Handle(AddToCartCommand command)
        {
            var cart = await _uow.Carts.GetByUserIdAsync(command.UserId)
                       ?? new Cart { UserId = command.UserId };

            var product = await _uow.Products.GetBySkuAsync(command.Dto.SKU);
            if (product == null)
                return ApiResponse<string>.Fail("Product not found");

            if (product.StockQuantity < command.Dto.Quantity)
                return ApiResponse<string>.Fail("Not enough stock");

            var existingItem = await _uow.Carts.GetCartItemAsync(cart.Id, command.Dto.SKU);
            if (existingItem != null)
            {
                existingItem.Quantity += command.Dto.Quantity;
                _uow.Carts.UpdateCartItem(existingItem);
            }
            else
            {
                var item = new CartItem
                {
                    CartId = cart.Id,
                    SKU = command.Dto.SKU,
                    Quantity = command.Dto.Quantity
                };
                await _uow.Carts.AddCartItemAsync(item);
            }

            if (cart.Id == Guid.Empty) _uow.Carts.UpdateCart(cart);
            await _uow.SaveChangesAsync();

            return ApiResponse<string>.Ok("Product added to cart successfully");
        }
    }
}
