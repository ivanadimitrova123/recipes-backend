using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public string ProfilePicture { get; set; }
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    
}
