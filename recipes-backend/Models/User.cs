using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace recipes_backend.Models;

public class User
{
    [Key]
    public long Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [ForeignKey("PictureId")]
    public long? ProfilePictureId { get; set; }
    [JsonIgnore]
    public Picture? ProfilePicture { get; set; }
    [JsonIgnore]
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    
    // Users that the current user is following
    public ICollection<User> Following { get; set; } = new List<User>();
    [JsonIgnore]

    // Users who are following the current user
    public ICollection<User> Followers { get; set; } = new List<User>();

    //Recepies that the user has saved
    public ICollection <UserSavedRecipe>SavedRecepies { get; set; }

    public ICollection<UserGrades> UsersGrades { get; set; }

}
