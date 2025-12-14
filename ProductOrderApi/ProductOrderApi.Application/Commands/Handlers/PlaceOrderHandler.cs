using ProductOrderApi.Domain.Entities;

namespace ProductOrderApi.Application.Commands.Handlers
{
    public class PlaceOrderHandler
    {
        private readonly IUnitOfWork _uow;
        public PlaceOrderHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<Guid>> Handle(PlaceOrderCommand command)
        {
            using var transaction = await _uow.DbContext.Database.BeginTransactionAsync();
            var order = new Order { UserId = command.UserId };
            decimal total = 0;

            foreach (var item in command.Items)
            {
                var product = await _uow.Products.GetBySkuAsync(item.SKU);
                if (product == null)
                    return ApiResponse<Guid>.Fail($"Product {item.SKU} does not exist");

                if (product.StockQuantity < item.Quantity)
                    return ApiResponse<Guid>.Fail($"Not enough stock for {item.SKU}");

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
            await _uow.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<Guid>.Ok(order.Id, "Order placed successfully");
        }
    }
}
