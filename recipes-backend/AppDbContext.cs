using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models;

namespace recipes_backend
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet for your custom User class
        public DbSet<User> Users { get; set; }

        // DbSet for your Recipe class
        public DbSet<Recipe> Recipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity relationships for Recipe and User
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.User)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.UserId);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}