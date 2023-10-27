using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace recipes_backend.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    

    public AccountController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }
    
    [HttpGet("current")]
    [Authorize]
    public IActionResult GetCurrentUserInfo()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return NotFound("User not found");
        }

        if (long.TryParse(userIdClaim.Value, out long userId))
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return Ok(user);
            }
        }
        return NotFound("User not found or conversion failed.");
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(User model)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email is already in use.");
                return BadRequest(ModelState);
            }
            // Hash the user's password
            model.Password = _passwordHasher.HashPassword(model, model.Password);

            // Add the user to the database
            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }

        return BadRequest(ModelState);
    }
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Username);
    
        if (user == null)
        {
            return BadRequest("Invalid username or password.");
        }

        // Check if the password matches (You should use a secure password hashing method).
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            return BadRequest("Invalid username or password.");
        }

        // Generate the JWT token.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Add the user's ID as a claim.
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VkVSfGYr8VSkxDRF8ftKCwZuqN1lLLxBZN7s20jS"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7222/",
            audience: "https://localhost:7222/",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30), // Set token expiration time.
            signingCredentials: creds
        );
            
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Return the JWT token as part of the response.
        return Ok(new
        {
            Token = tokenString,
        });

        //return Ok("Login successful.");
    }



}
