using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Domain.Entities;
using ProductOrderApi.Infrastructure.Persistence;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context) => _context = context;

    public async Task<Category?> GetByIdAsync(Guid id) => await _context.Categories.FindAsync(id);

    public async Task<IEnumerable<Category>> GetAllAsync() => await _context.Categories.ToListAsync();

    public async Task AddAsync(Category category) => await _context.Categories.AddAsync(category);

    public void Update(Category category) => _context.Categories.Update(category);

    public void Delete(Category category)
    {
        category.IsDeleted = true;
        _context.Categories.Update(category);
    }
}
