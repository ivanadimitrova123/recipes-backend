namespace recipes_backend.Models.Dto
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string Role { get; set; }

        public UserDto(long id, string username, string firstName, string lastName, string email, string picture, string role)
        {
            Id = id;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Picture = picture;
            Role = role;
        }
    }   
}
