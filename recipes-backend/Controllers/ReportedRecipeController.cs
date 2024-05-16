using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models;
using recipes_backend.Models.Dto;

namespace recipes_backend.Controllers
{
    [Route("api/reportedrecipe")]
    [ApiController]
    public class ReportedRecipeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportedRecipeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetReportedRecipes()
        {
            var allReportedRecipes = _context.ReportedRecipes.Include(r=>r.Recipe).ToList();
            List<object>reprtedRecipes = new List<object>();
            foreach (var recipe in allReportedRecipes)
            {
                string img = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipe.Recipe.PictureId}";
                reprtedRecipes.Add(new
                {
                    recipe.RecipeId,
                    recipe.Recipe.Name,
                    img,
                });
            }

            return Ok(reprtedRecipes);
        }

        [HttpPost]
        public async Task<IActionResult> ReportComment([FromForm] long userId, [FromForm] long recipeId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Id == recipeId);
            if (recipe == null)
            {
                return BadRequest("Comment does not exist");
            }

            var reportedRecipe = _context.ReportedRecipes.FirstOrDefault(rr => rr.RecipeId == recipeId && rr.UserId == userId);
            if (reportedRecipe != null)
            {
                return Ok("Already reported");
            }
            else
            {
                reportedRecipe = new ReportedRecipe
                {
                    UserId = userId,
                    RecipeId = recipeId
                };
            }



            _context.ReportedRecipes.Add(reportedRecipe);
            _context.SaveChanges();

            return Ok("Recipe Reported");
        }
    }
}
