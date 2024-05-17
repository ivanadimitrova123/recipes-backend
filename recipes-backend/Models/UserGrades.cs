using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace recipes_backend.Models
{
    public class UserGrades
    {
        public long UserId { get; set; }
        public long RecipeId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        public int Grade {  get; set; }

    }
}
