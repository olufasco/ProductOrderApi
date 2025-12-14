using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Domain.Entities;
using ProductOrderApi.Infrastructure.Persistence;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id) => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

    public void Update(User user) => _context.Users.Update(user);

    public void Delete(User user)
    {
        user.IsDeleted = true;
        _context.Users.Update(user);
    }
}
