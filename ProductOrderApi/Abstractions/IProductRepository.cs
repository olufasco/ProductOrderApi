using ProductOrderApi.Models;

namespace ProductOrderApi.Abstractions
{
    namespace ProductOrder.Application.Abstractions
    {
        public interface IProductRepository
        {
            Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
            Task<List<Product>> GetAllAsync(CancellationToken ct);
            Task AddAsync(Product product, CancellationToken ct);
            Task UpdateAsync(Product product, CancellationToken ct);
            Task SoftDeleteAsync(Product product, string deletedBy, CancellationToken ct);
        }
    }

}
