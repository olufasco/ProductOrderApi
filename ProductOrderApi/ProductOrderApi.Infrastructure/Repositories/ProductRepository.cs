using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Domain.Entities;
using ProductOrderApi.Infrastructure.Persistence;
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<Product?> GetByIdAsync(Guid id) => await _context.Products.FindAsync(id);

    public async Task<Product?> GetBySkuAsync(string sku) =>
        await _context.Products.FirstOrDefaultAsync(p => p.SKU == sku);

    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();

    public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);

    public void Update(Product product) => _context.Products.Update(product);

    public void Delete(Product product)
    {
        product.IsDeleted = true;
        _context.Products.Update(product);
    }
}
