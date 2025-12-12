using ProductOrderApi.Abstractions;
using ProductOrderApi.Abstractions.ProductOrder.Application.Abstractions;
using ProductOrderApi.Domain;
using ProductOrderApi.Models;
using System.Data;
using static ProductOrderApi.DTOs.OrderDtos;

namespace ProductOrderApi.Services
{
    public class OrderService
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICartRepository _cartRepo; // add a cart repo abstraction

        public OrderService(
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            IUnitOfWork uow,
            ICartRepository cartRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _uow = uow;
            _cartRepo = cartRepo;
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var order = await _orderRepo.GetByIdAsync(id, ct);
            return order is null ? null : MapToDto(order);
        }

        public async Task<List<OrderDto>> GetAllAsync(CancellationToken ct)
        {
            var orders = await _orderRepo.GetAllAsync(ct);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderDto> PlaceOrderAsync(PlaceOrderRequest request, CancellationToken ct)
        {
            Order? createdOrder = null;

            await _uow.ExecuteInTransactionAsync(async _ =>
            {
                var cart = await _cartRepo.GetByIdAsync(request.CartId, ct)
                    ?? throw new DomainException("CART_NOT_FOUND", $"Cart {request.CartId} not found");

                var products = new List<Product>();

                foreach (var item in cart.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId, ct)
                        ?? throw new DomainException("PRODUCT_NOT_FOUND", $"Product {item.ProductId} not found");

                    if (product.StockQuantity < item.Quantity)
                        throw new DomainException(ErrorCodes.InsufficientStock, $"Insufficient stock for {product.Name}");

                    product.StockQuantity -= item.Quantity;
                    await _productRepo.UpdateAsync(product, ct);
                    products.Add(product);
                }

                createdOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = cart.UserId,
                    CustomerName = request.CustomerName,
                    OrderLines = cart.Items.Select(i =>
                    {
                        var product = products.Single(p => p.Id == i.ProductId);
                        return new OrderLine
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            Quantity = i.Quantity,
                            UnitPrice = product.Price,
                            LineTotal = product.Price * i.Quantity,
                            Sku = product.Sku,
                            Name = product.Name
                        };
                    }).ToList(),
                    TotalAmount = cart.Items.Sum(i =>
                    {
                        var product = products.Single(p => p.Id == i.ProductId);
                        return product.Price * i.Quantity;
                    })
                };

                await _orderRepo.AddAsync(createdOrder, ct);
                await _uow.SaveChangesAsync(ct);

                // Optionally clear the cart after checkout
                await _cartRepo.DeleteAsync(cart, ct);

            }, ct, IsolationLevel.Serializable);

            return MapToDto(createdOrder!);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var order = await _orderRepo.GetByIdAsync(id, ct);
            if (order is null) return false;

            await _orderRepo.SoftDeleteAsync(order, "system", ct);
            await _uow.SaveChangesAsync(ct);
            return true;
        }

        private static OrderDto MapToDto(Order order) =>
            new OrderDto(
                order.Id,
                order.UserId,
                order.CustomerName,
                order.TotalAmount,
                order.OrderLines.Select(l =>
                    new OrderLineDto(l.Id, l.ProductId, l.Sku, l.Name, l.Quantity, l.UnitPrice, l.LineTotal)
                ).ToList()
            );
    }
}