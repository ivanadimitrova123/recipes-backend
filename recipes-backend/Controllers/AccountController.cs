﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using recipes_backend.Models.Dto;


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
    
    [Authorize(Roles = "Admin")]
    [HttpGet("allUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    [HttpGet("search")]
    public IActionResult SearchUser(string text)
    {
        var users = _context.Users.Where(u => u.Username.Contains(text)).Take(3).ToList();
        List<UserDto> userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            string img = "";
            if (user.ProfilePictureId != null)
                img = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{user.ProfilePictureId}";
            userDtos.Add(new UserDto(user.Id, user.Username,user.FirstName,user.LastName,user.Email,img,user.Role));
        }
        return Ok(userDtos);
    }

    
    [HttpGet("user/{userId}")]
    public IActionResult GetUserProfile(long userId)
    {
        // Retrieve the user's profile by their user ID
        var userProfile = _context.Users
            .Include(u => u.Following)
            .Include(u => u.Followers)
            .Include(u=>u.Recipes)
            .Include(u => u.ProfilePicture)
            .FirstOrDefault(u => u.Id == userId);

        if (userProfile == null)
        {
            return NotFound("User not found.");
        }
        // Create an anonymous object to send only the necessary data
        string userImage = null;
        if (userProfile.ProfilePictureId != null) { userImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{userProfile.ProfilePictureId}"; }
        var userData = new
        {
            Id = userProfile.Id,
            Username = userProfile.Username,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            imageId = userProfile.ProfilePictureId,
            userImage = userImage,
            Recipes = userProfile.Recipes.Select(recipe => new
            {   
                recipe.Id,
                recipe.Name,
                recipe.PictureId,
                RecipeImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipe.PictureId}",
                Comments = _context.Comments.Where(c => c.RecipeId == recipe.Id).Count(),
                recipe.Rating

            }).ToList(),
            Following=userProfile.Following.Count,
            Followers=userProfile.Followers.Count
        };

        

        return Ok(userData);
    }

    
    [HttpGet("current")]
   
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
                .Include(u => u.Following)
                .Include(u => u.Followers)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Recipes)
                .FirstOrDefault(u => u.Id == userId);


            /*
             * 
            if (user.ProfilePictureId == null)
            {
                user.ProfilePictureId = 1;
            }*/

            if (user != null)
            {
                var userData = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.ProfilePictureId,
                    /*
                    ProfilePicture = new
                    {
                        user.ProfilePictureId
                    },
                    */
                    UserImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{user.ProfilePictureId}",
                    Recipes = user.Recipes.Select(recipe => new
                    {   
                        recipe.Id,
                        recipe.Name,
                        recipe.PictureId,
                        RecipeImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipe.PictureId}",
                        recipe.Rating


                    }).ToList(),
                    Following=user.Following.Count,
                    Followers=user.Followers.Count
                };
                
               return Ok(userData);
               
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

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username is already in use");
                return BadRequest(ModelState);
            }

            model.ProfilePictureId = null;
            // Hash the user's password
            model.Password = _passwordHasher.HashPassword(model, model.Password);

            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }

        return BadRequest(ModelState);
    }
    
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model.Username.IsNullOrEmpty() || model.Password.IsNullOrEmpty()) 
        {
            return BadRequest("Enter Username and Password");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Username);
    
        if (user == null)
        {
            return BadRequest("Invalid username or password.");
        }

        
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            return BadRequest("Invalid username or password.");
        }

        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
            new Claim(ClaimTypes.Role, user.Role),

        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VkVSfGYr8VSkxDRF8ftKCwZuqN1lLLxBZN7s20jS"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7222/",
            audience: "https://localhost:7222/",
            claims: claims,
            expires: DateTime.Now.AddDays(7), 
            signingCredentials: creds
        );
        string img = "";
        if (user.ProfilePictureId != null)
            img = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{user.ProfilePictureId}";
        UserDto userDto = new UserDto(user.Id, user.Username, user.FirstName, user.LastName, user.Email, img, user.Role);
            
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { User = userDto, Token = tokenString, });
    }
}
