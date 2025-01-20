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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<UserRole>().ToTable("user_role");
            modelBuilder.Entity<Password>().ToTable("password");
            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<Product>().ToTable("product");
            

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