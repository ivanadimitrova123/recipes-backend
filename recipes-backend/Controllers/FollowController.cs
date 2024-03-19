namespace recipes_backend.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/follow")]
[ApiController]
public class FollowController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FollowController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("status/{followedUserId}")]
    public IActionResult IsFollowed(long followedUserId)
    {
        
        var userId = GetUserId();
        var user = _context.Users
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefault(u => u.Id == userId);


        foreach (var u in user.Following)
        {
            if(u.Id == followedUserId)
            {
                return Ok(true);
            }
        }
        return Ok(false);
    }

    [HttpPost("{followedUserId}")]
    public IActionResult FollowUser(long followedUserId)
    {
        // Get the authenticated user's ID
        var userId = GetUserId();
        var user = _context.Users
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }
        if (user.Id == followedUserId)
        {
            return BadRequest("You cannot follow yourself.");
        }
        var followedUser = _context.Users
            .Include(u => u.Following)
            .FirstOrDefault(u => u.Id == followedUserId);

        if (followedUser == null)
        {
            return NotFound("User to be followed not found.");
        }
        if (user.Following.Contains(followedUser))
        {
            return BadRequest("You are already following this user.");
        }

        // Follow the user
        user.Following.Add(followedUser);
        followedUser.Followers.Add(user);
        _context.SaveChanges();

        return Ok("You are now following this user.");
    }

    [HttpDelete("{followedUserId}")]
    public IActionResult UnfollowUser(long followedUserId)
    {
        // Get the authenticated user's ID
        var userId = GetUserId();
        var user = _context.Users
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }
        var followedUser = _context.Users
            .Include(u => u.Following)
            .FirstOrDefault(u => u.Id == followedUserId);

        if (followedUser == null)
        {
            return NotFound("User to be unfollowed not found.");
        }
        if (!user.Following.Contains(followedUser))
        {
            return BadRequest("You are not currently following this user.");
        }

        // Unfollow the user
        user.Following.Remove(followedUser);
        followedUser.Followers.Remove(user);
        _context.SaveChanges();

        return Ok("You have unfollowed this user.");
    }

    [HttpGet("following")]
    public IActionResult GetFollowingUsers()
    {
        // Get the authenticated user's ID
        var userId = GetUserId();
        var user = _context.Users
            .Include(u => u.Following)
            .Include(u=>u.ProfilePicture)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Retrieve the users that the current user is following
        var followingUsers = user.Following.ToList();
        return Ok(followingUsers);
    }

    [HttpGet("recipes")]
    public IActionResult GetRecipesOfFollowedUsers()
    {
        // Get the authenticated user's ID
        var userId = GetUserId();

        // Retrieve the user from the database, including the recipes of the users they follow
        var user = _context.Users
            .Include(u => u.Following)
            .ThenInclude(f => f.Recipes)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }
        // Create a list to store the recipes and their associated user's images
        var recipesWithImages = new List<object>();

        foreach (var followedUser in user.Following)
        {
            foreach (var recipe in followedUser.Recipes)
            {
                var followedUserImage = _context.Pictures.Find(followedUser.ProfilePictureId);
                var recipeImage = _context.Pictures.Find(recipe.PictureId);

                // Add the recipe and its associated user's image to the list
                recipesWithImages.Add(new
                {
                    recipe = recipe,
                    userImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{followedUserImage.Id}",
                    recipeImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipeImage.Id}"
                });
            }
        }

        return Ok(recipesWithImages);
    }



    private long GetUserId()
    {
        // Get the authenticated user's ID from the JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
        {
            throw new Exception("User not found or JWT token is invalid.");
        }

        return userId;
    }
}