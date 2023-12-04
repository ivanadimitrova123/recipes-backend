using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend;
using recipes_backend.Models;

namespace recipes_backend.Controllers
{
    [Route("api/saverecipe")]
    [ApiController]
    public class UserSavedRecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserSavedRecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("save")]
        public async Task<IActionResult>SaveRecipe(long recipeId, long userId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("You must be logged in to create a recipe.");
            }

            User user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return BadRequest("No user with that id");
            }

            Recipe recipe = _context.Recipes.SingleOrDefault(r => r.Id == recipeId);
            if (recipe == null)
            {
                return BadRequest("No recipe with that id");
            }

            var tmp = await _context.UserSavedRecipe.SingleOrDefaultAsync(x => x.UserId == userId && x.RecipeId == recipeId);
            if (tmp != null) 
            {
                return BadRequest("Already Saved");
            }

            var saveRecipe = new UserSavedRecipe
            {
                UserId = userId,
                RecipeId = recipeId
            };

            _context.UserSavedRecipe.Add(saveRecipe);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult>GetSavedRecipes(long userId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("You must be logged in to create a recipe.");
            }

            User user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return BadRequest("No user with that id");
            }

            var usrList = await _context.UserSavedRecipe.Where(usr => usr.UserId == userId).ToListAsync();

            List<Recipe> recipes = new List<Recipe>();

            foreach (var usrItem in usrList)
            {
                Recipe recipe = await _context.Recipes.SingleOrDefaultAsync(r => r.Id == usrItem.RecipeId);
                recipes.Add(recipe);
            }

            return Ok(recipes);
        }
    }
}
