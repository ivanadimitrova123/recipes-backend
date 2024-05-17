using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace recipes_backend.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int? ParentId { get; set; }
        [ForeignKey("UserId")]
        public long UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        [ForeignKey("RecipeId")]
        public long RecipeId { get; set; }
        [JsonIgnore]
        public Recipe? Recipe { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime DateCreated {  get; set; } = DateTime.Now.ToUniversalTime();
        public Comment? Parent { get; set; }
        public ICollection<Comment>? Children { get; set;}
    }
}
