﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using recipes_backend.Models;

namespace recipes_backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models.Dto;
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

    [HttpGet("popular")]
    public IActionResult GetPopularRecipes()
    {
        
      

        // Fetch recipes created by the user with the matching user ID
        var recipes = _context.Recipes.Include(r => r.User).Take(7).ToList();
        List<object> editedRecipes = new List<object>();
        foreach (var recipe in recipes)
        {
            string img = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipe.PictureId}";
            editedRecipes.Add(new { recipe.Id, recipe.Name, img, recipe.Total, recipe.Level, recipe.User.Username });
        }

        return Ok(editedRecipes);
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
        var recipe = _context.Recipes
            .Include(r => r.Picture)
            .Include(r=>r.User)
            .FirstOrDefault(r => r.Id == id);


        if (recipe == null)
        {
            return NotFound("Recipe not found");
        }

        List<String>ingridients =  recipe.Ingredients[0].Replace("\r","").Split("\n").ToList();
        recipe.Ingredients = ingridients;

        string image = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipe.User.ProfilePictureId}";

        return Ok(new { Recipe = recipe, recipeUserImage = image });
    }

    [HttpGet("search")]
    public IActionResult SearchRecipe(String text)
    {
        List<SearchRecipeDto> recipeDtos = new List<SearchRecipeDto>();
        var recipes = _context.Recipes.Include(r => r.Picture).Where(r => r.Name.Contains(text)).Take(3).ToList();
        foreach(var recipe in recipes)
        {
            string img = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipe.PictureId}";
            recipeDtos.Add(new SearchRecipeDto(recipe.Id, recipe.Name, img));
        }
        return Ok(recipeDtos);
    }


    [HttpPost]
    public IActionResult CreateRecipe([FromForm] Recipe recipe, IFormFile photo, [FromForm] List<long> selectedCategoryIds)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized("You must be logged in to create a recipe.");
        }
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
        {
            return Unauthorized("User not found or conversion failed.");
        }
        byte[] photoData = null;
        string photoContentType = null;

        if (photo != null && photo.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                photo.CopyTo(memoryStream);
                photoData = memoryStream.ToArray();
                photoContentType = photo.ContentType;
            }
        }
        Picture pic = new Picture();
        pic.ImageData = photoData;
        pic.ContentType = photoContentType;
        pic.FileName = "";
        _context.Pictures.Add(pic);
        _context.SaveChanges();
        pic.FileName = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{pic.Id}";
        recipe.Picture = pic;
        recipe.PictureId = pic.Id;
        recipe.UserId = userId;
        foreach (var categoryId in selectedCategoryIds)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                recipe.Categories.Add(category);
            }
        }
        _context.Recipes.Add(recipe);
        _context.SaveChanges();
        return Ok();
    }
    
    [HttpPut("{id}")]
    public IActionResult EditRecipe(long id, [FromForm] Recipe updatedRecipe,IFormFile? photo, [FromForm] List<long> selectedCategoryIds)
    {
        // Find the recipe by its ID
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe == null)
        {
            return NotFound("Recipe not found");
        }

        // Check if the user is authorized to edit the recipe
        User user = _context.Users.FirstOrDefault(u => u.Id == GetUserIdFromClaims());
        if (user.Role != "Admin") 
        {
            if (recipe.UserId != GetUserIdFromClaims())
            {
                return Forbid("You are not authorized to edit this recipe.");
            }
        }

        if (photo != null)
        {
            Picture pic = new Picture();

            byte[] photoData = null;
            string photoContentType = null;

            if (photo != null && photo.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    photo.CopyTo(memoryStream);
                    photoData = memoryStream.ToArray();
                    photoContentType = photo.ContentType;
                }
                pic.ImageData = photoData;
                pic.ContentType = photoContentType;
                pic.FileName = "";
                _context.Pictures.Add(pic);
                _context.SaveChanges();
                pic.FileName = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{pic.Id}";
                recipe.Picture = pic;
                recipe.PictureId = pic.Id;
            }
        }

        
        
        
        // Update the recipe properties
        recipe.Name = updatedRecipe.Name;
        recipe.Description = updatedRecipe.Description;
       
        recipe.Ingredients = updatedRecipe.Ingredients;
        recipe.Level = updatedRecipe.Level;
        recipe.Cook = updatedRecipe.Cook;
        recipe.Prep = updatedRecipe.Prep;
        recipe.Total = updatedRecipe.Total;
        recipe.Yield = updatedRecipe.Yield;



        recipe.Categories.Clear();
        foreach (var categoryId in selectedCategoryIds)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                recipe.Categories.Add(category);
            }
        }
        
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
        User user = _context.Users.FirstOrDefault(u => u.Id == GetUserIdFromClaims());
        if(user.Role != "Admin")
        {
            if (recipe.UserId != GetUserIdFromClaims())
            {
                return Forbid("You are not authorized to delete this recipe.");
            }
        }
      
        
       
        Picture i = _context.Pictures.FirstOrDefault(i => i.Id == recipe.PictureId);
        _context.Pictures.Remove(i); 
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