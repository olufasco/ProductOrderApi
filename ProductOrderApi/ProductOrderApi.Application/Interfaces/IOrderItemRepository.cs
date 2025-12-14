using ProductOrderApi.Domain.Entities;

namespace ProductOrderApi.API.ProductOrderApi.Application.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem?> GetByIdAsync(Guid orderItemId);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
        Task AddAsync(OrderItem orderItem);
        void Delete(OrderItem orderItem);
    }
}
