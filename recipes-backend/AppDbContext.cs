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

            modelBuilder.Entity<UserGrades>()
                .HasKey(ug => new { ug.UserId, ug.RecipeId });
          
            modelBuilder.Entity<UserGrades>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UsersGrades)
                .HasForeignKey(ug =>  ug.UserId);

            modelBuilder.Entity<UserGrades>()
                .HasOne(ug => ug.Recipe)
                .WithMany(r => r.UsersGrades)
                .HasForeignKey(ug => ug.RecipeId);

            modelBuilder.Entity<UserSavedRecipe>()
                .HasKey(usr => new {usr.UserId, usr.RecipeId});

            modelBuilder.Entity<UserSavedRecipe>()
                .HasOne(usr => usr.User)
                .WithMany(u => u.SavedRecepies)
                .HasForeignKey(usr =>  usr.UserId);

            modelBuilder.Entity<UserSavedRecipe>()
                .HasOne(usr => usr.Recipe)
                .WithMany(r => r.SavedRecepies)
                .HasForeignKey(usr => usr.RecipeId);

            modelBuilder.Entity<Comment>(comment =>
            {
                comment.HasKey(c => c.CommentId);
                comment.HasIndex(c => c.ParentId);

                comment.HasOne(c => c.Parent)
                    .WithMany(c => c.Children)
                    .HasForeignKey(c => c.ParentId);
            });
             
                

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<UserSavedRecipe>UserSavedRecipe { get; set; }


        public DbSet<UserGrades>UserGrades { get; set; }
        public DbSet<Comment>Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}