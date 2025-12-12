using ProductOrderApi.Abstractions.ProductOrder.Application.Abstractions;
using System.Data;

namespace ProductOrderApi.Abstractions
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        ICartRepository Carts { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> action,
            CancellationToken ct,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}