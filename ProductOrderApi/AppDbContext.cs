using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Models;

namespace ProductOrderApi
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUser)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Global query filters for soft delete
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            modelBuilder.Entity<OrderLine>().HasQueryFilter(l => !l.IsDeleted);

            // 🔹 Decimal precision for money values
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderLine>().Property(l => l.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<OrderLine>().Property(l => l.LineTotal).HasPrecision(18, 2);

            // 🔹 Concurrency token
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<byte[]>("RowVersion")
                        .IsRowVersion();
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = _currentUser.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = _currentUser.UserId;
                        break;

                    case EntityState.Deleted:
                        // 🔹 Convert hard delete into soft delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        entry.Entity.DeletedBy = _currentUser.UserId;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}