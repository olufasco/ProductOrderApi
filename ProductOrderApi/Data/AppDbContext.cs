using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Models;

namespace ProductOrderApi.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUser)
            : base(options)
        {
            _currentUser = currentUser;
        }

        // DbSets
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Picture> Pictures => Set<Picture>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //  Global query filters for soft delete
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Picture>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Cart>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<CartItem>().HasQueryFilter(ci => !ci.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            modelBuilder.Entity<OrderLine>().HasQueryFilter(ol => !ol.IsDeleted);

            //  Relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Pictures)
                .WithOne(pic => pic.Product)
                .HasForeignKey(pic => pic.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderLines)
                .WithOne(ol => ol.Order)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔹 Decimal precision for money values
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderLine>().Property(l => l.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<OrderLine>().Property(l => l.LineTotal).HasPrecision(18, 2);
            modelBuilder.Entity<Cart>().Property(c => c.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<CartItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<CartItem>().Property(i => i.LineTotal).HasPrecision(18, 2);

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
                        //  Convert hard delete into soft delete
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