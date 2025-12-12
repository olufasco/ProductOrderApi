using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Abstractions.ProductOrder.Application.Abstractions;
using ProductOrderApi.Data;
using ProductOrderApi.Models;

namespace ProductOrderApi.Repositories
{
        public class ProductRepository : IProductRepository
        {
            private readonly AppDbContext _db;
            public ProductRepository(AppDbContext db) => _db = db;

            public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) =>
                _db.Products.Include(p => p.Category)
                            .Include(p => p.Pictures)
                            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

            public Task<List<Product>> GetAllAsync(CancellationToken ct) =>
                _db.Products.Where(p => !p.IsDeleted)
                            .Include(p => p.Category)
                            .Include(p => p.Pictures)
                            .AsNoTracking()
                            .ToListAsync(ct);

            public async Task AddAsync(Product product, CancellationToken ct) =>
                await _db.Products.AddAsync(product, ct);

            public Task UpdateAsync(Product product, CancellationToken ct)
            {
                _db.Products.Update(product);
                return Task.CompletedTask;
            }

            public Task SoftDeleteAsync(Product product, string deletedBy, CancellationToken ct)
            {
                product.IsDeleted = true;
                product.DeletedAt = DateTime.UtcNow;
                product.DeletedBy = deletedBy;
                _db.Products.Update(product);
                return Task.CompletedTask;
            }
        }
}
