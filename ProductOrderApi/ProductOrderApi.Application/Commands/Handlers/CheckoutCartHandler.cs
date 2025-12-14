using ProductOrderApi.Domain.Entities;

namespace ProductOrderApi.Application.Commands.Handlers
{
    public class CheckoutCartHandler
    {
        private readonly IUnitOfWork _uow;
        public CheckoutCartHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<Guid>> Handle(CheckoutCartCommand command)
        {
            var cart = await _uow.Carts.GetByUserIdAsync(command.UserId);
            if (cart == null || !cart.Items.Any())
                return ApiResponse<Guid>.Fail("Cart is empty");

            using var transaction = await _uow.DbContext.Database.BeginTransactionAsync();
            var order = new Order { UserId = command.UserId };
            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var product = await _uow.Products.GetBySkuAsync(item.SKU);
                if (product == null || product.StockQuantity < item.Quantity)
                    return ApiResponse<Guid>.Fail($"Product {item.SKU} out of stock");

                product.StockQuantity -= item.Quantity;
                _uow.Products.Update(product);

                order.Items.Add(new OrderItem
                {
                    SKU = item.SKU,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                total += item.Quantity * product.Price;
            }

            order.TotalAmount = total;
            await _uow.Orders.AddAsync(order);

            // Clear cart items
            _uow.DbContext.CartItems.RemoveRange(cart.Items);

            await _uow.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<Guid>.Ok(order.Id, "Order placed successfully");
        }
    }
}
