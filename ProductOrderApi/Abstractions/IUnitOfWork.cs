using System.Data;

namespace ProductOrderApi.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> action,
            CancellationToken ct,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }

}
