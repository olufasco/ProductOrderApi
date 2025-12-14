using ProductOrderApi.API.ProductOrderApi.Application.Interfaces;
using ProductOrderApi.Infrastructure.Persistence;
public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    IOrderItemRepository OrderItems { get; }

    AppDbContext DbContext { get; }
    Task<int> SaveChangesAsync();
}
