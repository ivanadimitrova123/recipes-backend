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
        public DbSet<Picture> Pictures { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity relationships for Recipe and User
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.User)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.UserId);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Followers)
                .WithMany(u => u.Following);
            
            // Configure the one-to-one relationship with ProfilePicture
            modelBuilder.Entity<User>()
                .HasOne(u => u.ProfilePicture)
                .WithOne()
                .HasForeignKey<User>(u => u.ProfilePictureId); 
            
            modelBuilder.Entity<Recipe>()
                .HasOne(u => u.Picture)
                .WithOne()
                .HasForeignKey<Recipe>(u => u.PictureId)
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }
    }
}