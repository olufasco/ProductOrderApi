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

            public OrderService(IProductRepository productRepo, IOrderRepository orderRepo, IUnitOfWork uow)
            {
                _productRepo = productRepo;
                _orderRepo = orderRepo;
                _uow = uow;
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
                    var products = new List<Product>();

                    foreach (var line in request.Lines)
                    {
                        var product = await _productRepo.GetByIdAsync(line.ProductId, ct)
                            ?? throw new DomainException("PRODUCT_NOT_FOUND", $"Product {line.ProductId} not found");

                        if (product.StockQuantity < line.Quantity)
                            throw new DomainException(ErrorCodes.InsufficientStock, $"Insufficient stock for {product.Name}");

                        product.StockQuantity -= line.Quantity;
                        await _productRepo.UpdateAsync(product, ct);
                        products.Add(product);
                    }

                    createdOrder = new Order
                    {
                        Id = Guid.NewGuid(),
                        Lines = request.Lines.Select(l =>
                        {
                            var product = products.Single(p => p.Id == l.ProductId);
                            return new OrderLine
                            {
                                Id = Guid.NewGuid(),
                                ProductId = product.Id,
                                Quantity = l.Quantity,
                                UnitPrice = product.Price
                            };
                        }).ToList(),
                        TotalAmount = request.Lines.Sum(l =>
                        {
                            var product = products.Single(p => p.Id == l.ProductId);
                            return product.Price * l.Quantity;
                        })
                    };

                    await _orderRepo.AddAsync(createdOrder, ct);
                    await _uow.SaveChangesAsync(ct);

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
                    order.CreatedAt,
                    order.TotalAmount,
                    order.Lines.Select(l => new OrderLineDto(l.ProductId, l.Quantity, l.UnitPrice, l.LineTotal)).ToList()
                );
        }
}
