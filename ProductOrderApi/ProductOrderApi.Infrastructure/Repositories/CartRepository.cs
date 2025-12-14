using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Domain.Entities;
using ProductOrderApi.Infrastructure.Persistence;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    public CartRepository(AppDbContext context) => _context = context;
    public async Task AddAsync(Cart cart) => await _context.Carts.AddAsync(cart);
    public async Task<Cart?> GetByUserIdAsync(Guid userId) =>
        await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<CartItem?> GetCartItemAsync(Guid cartId, string sku) =>
        await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cartId && i.SKU == sku);

    public async Task AddCartItemAsync(CartItem item) => await _context.CartItems.AddAsync(item);

    public void UpdateCartItem(CartItem item) => _context.CartItems.Update(item);
    public void DeleteCartItem(CartItem item) => _context.CartItems.Remove(item);
    public void RemoveCartItem(CartItem item) => _context.CartItems.Remove(item);

    public void UpdateCart(Cart cart) => _context.Carts.Update(cart);
}
