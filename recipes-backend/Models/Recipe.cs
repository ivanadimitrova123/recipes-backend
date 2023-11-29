using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace recipes_backend.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("PictureId")]
        public long? PictureId { get; set; }

        [JsonIgnore]
        public Picture? Picture { get; set; }
        public List<string> Ingredients { get; set; }
        
        [ForeignKey("UserId")]
        public long UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        public string Level {  get; set; }
        public string Prep { get; set; }
        public string Cook {  get; set; }
        public string Total {  get; set; }
        public string Yield { get; set; }
        public float Rating { get; set; } = 0;

        public ICollection<UserGrades> UsersGrades { get; set; }
        public ICollection<UserSavedRecipe> SavedRecepies { get; set; }

    }
}