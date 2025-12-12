using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Data;
using ProductOrderApi.Models;
using static ProductOrderApi.DTOs.CartDtos;

namespace ProductOrderApi.Services
{
    public class CartService
    {
        private readonly AppDbContext _db;
        public CartService(AppDbContext db) => _db = db;

        public async Task<Cart> GetOrCreateCartAsync(Guid userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
            {
                cart = new Cart { UserId = userId };
                await _db.Carts.AddAsync(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<Cart> AddItemAsync(Guid userId, AddToCartDto dto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var product = await _db.Products.FindAsync(dto.ProductId)
                ?? throw new InvalidOperationException("Product not found");

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId && i.Sku == dto.Sku);
            if (existing is null)
            {
                var item = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Sku = dto.Sku,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = product.Price * dto.Quantity
                };
                cart.Items.Add(item);
            }
            else
            {
                existing.Quantity += dto.Quantity;
                existing.LineTotal = existing.UnitPrice * existing.Quantity;
            }

            cart.TotalAmount = cart.Items.Sum(i => i.LineTotal);
            await _db.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateItemAsync(Guid userId, UpdateCartItemDto dto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == dto.CartItemId)
                ?? throw new InvalidOperationException("Cart item not found");

            item.Quantity = dto.Quantity;
            item.LineTotal = item.UnitPrice * item.Quantity;
            cart.TotalAmount = cart.Items.Sum(i => i.LineTotal);
            await _db.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RemoveItemAsync(Guid userId, Guid cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
                ?? throw new InvalidOperationException("Cart item not found");

            cart.Items.Remove(item);
            cart.TotalAmount = cart.Items.Sum(i => i.LineTotal);
            await _db.SaveChangesAsync();
            return cart;
        }
    }
}
