using ProductOrderApi.Domain.Entities;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task AddAsync(Category category);
    void Update(Category category);
    void Delete(Category category); // soft delete
}
