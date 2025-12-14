using ProductOrderApi.API.ProductOrderApi.Application.Interfaces;
using ProductOrderApi.Domain.Entities;
using ProductOrderApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ProductOrderApi.API.ProductOrderApi.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;
        public OrderItemRepository(AppDbContext context) => _context = context;

        public async Task<OrderItem?> GetByIdAsync(Guid orderItemId) =>
            await _context.OrderItems.FindAsync(orderItemId);

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId) =>
            await _context.OrderItems.Where(i => i.OrderId == orderId).ToListAsync();

        public async Task AddAsync(OrderItem orderItem) => await _context.OrderItems.AddAsync(orderItem);

        public void Delete(OrderItem orderItem) => _context.OrderItems.Remove(orderItem);
    }
}
