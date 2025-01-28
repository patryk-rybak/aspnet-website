using Microsoft.EntityFrameworkCore;

namespace shop.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<UserRole>().ToTable("user_role");
            modelBuilder.Entity<Password>().ToTable("password");
            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<Product>().ToTable("product");
            modelBuilder.Entity<Order>().ToTable("order");
            modelBuilder.Entity<OrderItem>().ToTable("order_item");
            
            // moge jeszcze pokierowac mapowaniem atrybutow z kals na kolumny 

            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.user_id, ur.role_id });
            
            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.user_id)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.role_id)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Password>()
            .HasKey(p => p.user_id);

            modelBuilder.Entity<User>()
            .HasKey(u => u.id);

            modelBuilder.Entity<Category>()
            .HasKey(c => c.id);

            modelBuilder.Entity<Order>()
                .HasKey(o => o.id);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.id);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.order_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.product_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.order_id, oi.product_id });

            // za cholere nei umiem ustawic tego password, ale dziala tez bez ustawiania 
            // modelBuilder.Entity<Product>()
            // .HasKey(p => p.id);

            // modelBuilder.Entity<Product>()
            // .HasForeignKey(p => p.category_id);

            // modelBuilder.Entity<Product>()
            // .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}