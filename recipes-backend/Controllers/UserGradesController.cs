using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
        public async Task<IActionResult> GradeRecipe([FromForm] long userId, [FromForm] long recipeId, [FromForm] int grade)
        {
            var test = userId;
            
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

            UserGrades userGrades = _context.UserGrades.SingleOrDefault(ug => ug.UserId == userId && ug.RecipeId == recipeId);
            if (userGrades == null)
            {
                var userGrade = new UserGrades
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    Grade = grade
                };

                _context.UserGrades.Add(userGrade);
                await _context.SaveChangesAsync();
            }
            else
            {
                userGrades.Grade = grade;
                await _context.SaveChangesAsync();
            }

    

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

            int reviews = await _context.UserGrades.Where(ug => ug.RecipeId == recipeId).CountAsync();

            UserGrades ug = await _context.UserGrades.SingleOrDefaultAsync(ug => ug.UserId == userId && ug.RecipeId == recipeId);

            if(ug == null)
            {
                return Ok(0);
            }

            return Ok(new { ug.Grade, reviews });

        }

        
    }
}
