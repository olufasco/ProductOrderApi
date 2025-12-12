using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Abstractions.ProductOrder.Application.Abstractions;
using ProductOrderApi.Data;
using System.Data;

namespace ProductOrderApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(
            AppDbContext db,
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            IUserRepository userRepo)
        {
            _db = db;
            Products = productRepo;
            Orders = orderRepo;
            Carts = cartRepo;
            Users = userRepo;
        }

        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public ICartRepository Carts { get; }
        public IUserRepository Users { get; }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            _db.SaveChangesAsync(ct);

        public async Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> action,
            CancellationToken ct,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await _db.Database.BeginTransactionAsync(isolationLevel, ct);
                await action(ct);
                await tx.CommitAsync(ct);
            });
        }
    }
}