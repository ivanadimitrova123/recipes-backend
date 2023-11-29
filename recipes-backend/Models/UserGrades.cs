using System.ComponentModel.DataAnnotations;

namespace recipes_backend.Models
{
    public class UserGrades
    {
        [Key]
        public int Id { get; set; }
        public long UserId { get; set; }
        public long RecipeId { get; set; }
        public User User { get; set; }
        public Recipe Recipe { get; set; }

        public int Grade {  get; set; }

    }
}
