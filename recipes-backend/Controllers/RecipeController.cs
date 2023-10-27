using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using recipes_backend.Models;

namespace recipes_backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/recipes")]
[Authorize]
public class RecipeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RecipeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetRecipes()
    {
        // Get the user's ID from the JWT token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
        {
            return BadRequest("User not found or JWT token is invalid.");
        }

        // Fetch recipes created by the user with the matching user ID
        var recipes = _context.Recipes
            .Where(r => r.UserId == userId)
            .ToList();

        return Ok(recipes);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetRecipeById(long id)
    {
        // Find the recipe by its ID
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound("Recipe not found");
        }

        return Ok(recipe);
    }


    [HttpPost]
    public IActionResult CreateRecipe([FromBody] Recipe recipe)
    {
        // Check if a user is authenticated
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized("You must be logged in to create a recipe.");
        }

        // Get the currently authenticated user's ID
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
        {
            return Unauthorized("User not found or conversion failed.");
        }
        
        recipe.UserId = userId;
        // Add the recipe to the database
        _context.Recipes.Add(recipe);
        _context.SaveChanges();

        // Return the created recipe in the HTTP response
        return CreatedAtAction("GetRecipeById", new { id = recipe.Id }, recipe);
    }
    
    [HttpPut("{id}")]
    public IActionResult EditRecipe(long id, [FromBody] Recipe updatedRecipe)
    {
        // Find the recipe by its ID
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound("Recipe not found");
        }

        // Check if the user is authorized to edit the recipe
        if (recipe.UserId != GetUserIdFromClaims())
        {
            return Forbid("You are not authorized to edit this recipe.");
        }

        // Update the recipe properties
        recipe.Name = updatedRecipe.Name;
        recipe.Description = updatedRecipe.Description;
        recipe.Picture = updatedRecipe.Picture;
        recipe.Ingredients = updatedRecipe.Ingredients;

        _context.SaveChanges();

        return Ok(recipe);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRecipe(long id)
    {
        // Find the recipe by its ID
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound("Recipe not found");
        }

        // Check if the user is authorized to delete the recipe
        if (recipe.UserId != GetUserIdFromClaims())
        {
            return Forbid("You are not authorized to delete this recipe.");
        }

        _context.Recipes.Remove(recipe);
        _context.SaveChanges();

        return NoContent();
    }

    private long GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
        {
            // You may want to handle this case differently, e.g., return an error response.
            throw new Exception("User not found or conversion failed.");
        }

        return userId;
    }


}