using ProductOrderApi.Domain.Entities;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Order order);
    void Delete(Order order);
}