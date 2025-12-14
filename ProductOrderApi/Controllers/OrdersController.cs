using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.API.ProductOrderApi.Application.DTOs;
using ProductOrderApi.Domain.Entities;
using System.Security.Claims;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public OrdersController(IUnitOfWork uow) => _uow = uow;

    private Guid GetUserId()
    {
        var claim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID in token.");
        return userId;
    }

    // Get all orders for the user
    [HttpGet]
    public async Task<IActionResult> GetUserOrders()
    {
        var orders = await _uow.Orders.GetByUserIdAsync(GetUserId());

        var dtos = orders.Select(o => new OrderDto
        {
            Id = o.Id,
            TotalAmount = o.TotalAmount,
            Items = o.Items.Select(i => new OrderItemDto
            {
                SKU = i.SKU,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        }).ToList();

        return Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(dtos));
    }

    // Add a new order
    [HttpPost]
    public async Task<IActionResult> AddOrder(CreateOrderDto dto)
    {
        var userId = GetUserId();

        if (dto.Items == null || !dto.Items.Any())
            return BadRequest(ApiResponse<string>.Fail("Order must have at least one item."));

        using var transaction = await _uow.DbContext.Database.BeginTransactionAsync();

        var order = new Order { UserId = userId };
        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var product = await _uow.Products.GetBySkuAsync(item.SKU);
            if (product == null)
                return BadRequest(ApiResponse<string>.Fail($"Product {item.SKU} not found"));

            if (product.StockQuantity < item.Quantity)
                return BadRequest(ApiResponse<string>.Fail($"Not enough stock for {item.SKU}"));

            product.StockQuantity -= item.Quantity;
            _uow.Products.Update(product);

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
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

        return Ok(ApiResponse<Guid>.Ok(order.Id, "Order placed successfully"));
    }

    // Delete an order
    [HttpDelete("{orderId:guid}")]
    public async Task<IActionResult> DeleteOrder(Guid orderId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order == null || order.UserId != GetUserId())
            return NotFound(ApiResponse<string>.Fail("Order not found"));

        _uow.Orders.Delete(order);
        await _uow.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Order deleted successfully"));
    }
}
