using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Data;
using ProductOrderApi.Models;

namespace ProductOrderApi.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _db;
        public CartRepository(AppDbContext db) => _db = db;

        public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken ct) =>
            await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task DeleteAsync(Cart cart, CancellationToken ct)
        {
            _db.Carts.Remove(cart);
            await _db.SaveChangesAsync(ct);
        }
    }
}