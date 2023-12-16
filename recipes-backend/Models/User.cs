using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace recipes_backend.Models;

public class User
{
    [Key]
    public long Id { get; set; }
    [Required(ErrorMessage = "Field can't be empty")]
    public string Username { get; set; }
    [Required(ErrorMessage = "Field can't be empty")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Field can't be empty")]
    [EmailAddress]
    public string Email { get; set; }
    [Required(ErrorMessage = "Field can't be empty")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Field can't be empty")]
    public string LastName { get; set; }
    [ForeignKey("PictureId")]
    public long? ProfilePictureId { get; set; }
    [JsonIgnore]
    public Picture? ProfilePicture { get; set; }
    [JsonIgnore]
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<User> Following { get; set; } = new List<User>();
    [JsonIgnore]
    public ICollection<User> Followers { get; set; } = new List<User>();
    public ICollection<UserSavedRecipe> SavedRecepies { get; set; } = new List<UserSavedRecipe>();
    public ICollection<UserGrades> UsersGrades { get; set; } = new List<UserGrades>();
    
    public string Role { get; set; } = "User";
}
