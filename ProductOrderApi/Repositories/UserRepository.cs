using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Data;
using ProductOrderApi.Models;

namespace ProductOrderApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
        }

        public void Remove(User user)
        {
            _db.Users.Remove(user);
        }
    }
}