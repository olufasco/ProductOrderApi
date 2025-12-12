using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Data;
using ProductOrderApi.DTOs;
using ProductOrderApi.Models;

namespace ProductOrderApi.Controllers
{
    [Route("api/[controller]")]
    public class CartController : BaseController
    {
        private readonly AppDbContext _db;
        public CartController(AppDbContext db) => _db = db;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return FailResponse<Cart>("Cart not found");

            var dto = new CartDtos.CartDto(
                cart.Id, cart.UserId, cart.TotalAmount,
                cart.Items.Select(i => new CartDtos.CartItemDto(
                    i.Id, i.ProductId, i.Sku, i.Product.Name, i.Quantity, i.UnitPrice, i.LineTotal
                )).ToList()
            );

            return OkResponse(dto);
        }

        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddItem(Guid userId, CartDtos.AddToCartDto dto)
        {
            var product = await _db.Products.FindAsync(dto.ProductId);
            if (product == null) return FailResponse<Cart>("Product not found");

            var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId)
                       ?? new Cart { UserId = userId };

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId && i.Sku == dto.Sku);
            if (existing == null)
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Sku = dto.Sku,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = product.Price * dto.Quantity
                });
            }
            else
            {
                existing.Quantity += dto.Quantity;
                existing.LineTotal = existing.UnitPrice * existing.Quantity;
            }

            cart.TotalAmount = cart.Items.Sum(i => i.LineTotal);

            _db.Carts.Update(cart);
            await _db.SaveChangesAsync();

            return OkResponse(cart, "Item added to cart");
        }
    }
}