using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Data;
using ProductOrderApi.DTOs;
using ProductOrderApi.Models;

namespace ProductOrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // protect all order endpoints
    public class OrdersController : BaseController
    {
        private readonly AppDbContext _db;
        public OrdersController(AppDbContext db) => _db = db;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(OrderDtos.CheckoutDto dto, CancellationToken ct)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == dto.CartId, ct);

            if (cart == null) return FailResponse<Order>("Cart not found");

            var order = new Order
            {
                UserId = cart.UserId,
                CustomerName = dto.CustomerName,
                TotalAmount = cart.TotalAmount,
                OrderLines = cart.Items.Select(i => new OrderLine
                {
                    ProductId = i.ProductId,
                    Sku = i.Sku,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = i.LineTotal
                }).ToList()
            };

            _db.Orders.Add(order);
            _db.Carts.Remove(cart); // clear cart after checkout
            await _db.SaveChangesAsync(ct);

            return OkResponse(order, "Order placed successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var orders = await _db.Orders
                .Include(o => o.OrderLines)
                .ToListAsync(ct);

            return OkResponse(orders, "Orders retrieved successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var order = await _db.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

            return order == null
                ? FailResponse<Order>("Order not found")
                : OkResponse(order, "Order retrieved successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrderDtos.UpdateOrderDto dto, CancellationToken ct)
        {
            var order = await _db.Orders.Include(o => o.OrderLines).FirstOrDefaultAsync(o => o.Id == id, ct);
            if (order == null) return FailResponse<Order>("Order not found");

            order.CustomerName = dto.CustomerName ?? order.CustomerName;
            order.Status = dto.Status ?? order.Status;

            await _db.SaveChangesAsync(ct);
            return OkResponse(order, "Order updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
            if (order == null) return FailResponse<Order>("Order not found");

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync(ct);

            return OkResponse("Order deleted successfully");
        }
    }
}