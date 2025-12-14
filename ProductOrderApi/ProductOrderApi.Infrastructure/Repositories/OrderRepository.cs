using ProductOrderApi.Domain.Entities;
using ProductOrderApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context) => _context = context;

    public async Task<Order?> GetByIdAsync(Guid orderId) =>
        await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId) =>
        await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ToListAsync();

    public async Task AddAsync(Order order) => await _context.Orders.AddAsync(order);

    public void Delete(Order order) => _context.Orders.Remove(order);
}
