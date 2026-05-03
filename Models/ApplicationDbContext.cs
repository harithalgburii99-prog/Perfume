using Microsoft.EntityFrameworkCore;

namespace PerfumeStore.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Perfume> Perfumes { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Ensures unique emails
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "Admin",
                Email = "admin@royalscent.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin"
            });

            // Seed Perfumes
            modelBuilder.Entity<Perfume>().HasData(
                new Perfume
                {
                    Id = 1,
                    Name = "Oud Supreme",
                    Description = "A rich blend of agarwood and dark spices, perfect for evening wear.",
                    Price = 129.99m,
                    ImagePath = "https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=600&q=80"
                },
                new Perfume
                {
                    Id = 2,
                    Name = "Midnight Rose",
                    Description = "Elegant damascus rose infused with subtle hints of vanilla and amber.",
                    Price = 89.50m,
                    ImagePath = "https://images.unsplash.com/photo-1592945403244-b3fbafd7f539?auto=format&fit=crop&w=600&q=80"
                },
                new Perfume
                {
                    Id = 3,
                    Name = "Citrus Horizon",
                    Description = "Fresh bergamot, lemon, and a touch of oceanic breeze. A vibrant daytime scent.",
                    Price = 75.00m,
                    ImagePath = "https://images.unsplash.com/photo-1588405748880-12d1d2a59f75?auto=format&fit=crop&w=600&q=80"
                }
            );
        }
    }
}
