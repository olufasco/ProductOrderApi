using ProductOrderApi.API.ProductOrderApi.Application.Interfaces;
using ProductOrderApi.Domain.Entities;
using ProductOrderApi.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public ICartRepository Carts { get; }
    public IOrderRepository Orders { get; }
    public IOrderItemRepository OrderItems { get; } 

    public AppDbContext DbContext => _context;

    public UnitOfWork(
        AppDbContext context,
        IUserRepository users,
        IProductRepository products,
        ICategoryRepository categories,
        ICartRepository carts,
        IOrderRepository orders,
        IOrderItemRepository orderItems)
    {
        _context = context;
        Users = users;
        Products = products;
        Categories = categories;
        Carts = carts;
        Orders = orders;
        OrderItems = orderItems;
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
