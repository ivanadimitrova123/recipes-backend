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
    [Route("api/usergrades")]
    [ApiController]
    public class UserGradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserGradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GradeRecipe(long recipeId,  long userId, int grade)
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

            Recipe recipe = _context.Recipes.SingleOrDefault(r =>  r.Id == recipeId);
            if (recipe == null)
            {
                return BadRequest("No recipe with that id");
            }

            var userGrade = new UserGrades
            {
                UserId = userId,
                RecipeId = recipeId,
                Grade = grade
            };

            _context.UserGrades.Add(userGrade);
            await _context.SaveChangesAsync();

            List<UserGrades> ug = await _context.UserGrades.Where(ug => ug.RecipeId == recipeId).ToListAsync();
            int sum = 0;
            foreach (var u in ug)
            {
                sum += u.Grade;
            }
            recipe.Rating = (float)sum / ug.Count;
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet]
        public async Task<IActionResult>HasGradedRecipe(long userId,long recipeId)
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

            UserGrades ug = await _context.UserGrades.SingleOrDefaultAsync(ug => ug.UserId == userId && ug.RecipeId == recipeId);

            if(ug == null)
            {
                return BadRequest("User has not graded this recipe");
            }

            return Ok(ug.Grade);

        }

        
    }
}
