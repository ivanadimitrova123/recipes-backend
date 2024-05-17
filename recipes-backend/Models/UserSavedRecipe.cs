using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace recipes_backend.Models
{
    public class UserSavedRecipe
    {
 
        public long UserId { get; set; }
        public long RecipeId {  get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public User User { get; set; }
        [ForeignKey("RecipeId")]
        [JsonIgnore]
        public Recipe Recipe { get; set; }


    }
}
