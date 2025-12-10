using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using System;
using System.Data;

namespace ProductOrderApi
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db) => _db = db;

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