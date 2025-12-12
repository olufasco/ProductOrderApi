using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Data;
using ProductOrderApi.Models;

namespace ProductOrderApi.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Order order, CancellationToken ct) =>
            await _db.Orders.AddAsync(order, ct);

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.Orders.Include(o => o.OrderLines)
                      .FirstOrDefaultAsync(o => o.Id == id, ct);

        public Task<List<Order>> GetAllAsync(CancellationToken ct) =>
            _db.Orders.Include(o => o.OrderLines)
                      .AsNoTracking()
                      .ToListAsync(ct);

        public Task UpdateAsync(Order order, CancellationToken ct)
        {
            _db.Orders.Update(order);
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(Order order, string deletedBy, CancellationToken ct)
        {
            order.IsDeleted = true;
            order.DeletedAt = DateTime.UtcNow;
            order.DeletedBy = deletedBy;
            _db.Orders.Update(order);
            return Task.CompletedTask;
        }

    }
}