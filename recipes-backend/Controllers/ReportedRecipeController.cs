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
            List<long>recipeIds = new List<long>();
            foreach (var recipe in allReportedRecipes)
            {
                if(!recipeIds.Contains(recipe.RecipeId))
                {
                    recipeIds.Add(recipe.RecipeId);
                    string img = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{recipe.Recipe.PictureId}";
                    reprtedRecipes.Add(new
                    {
                        recipe.RecipeId,
                        recipe.Recipe.Name,
                        img,
                    });
                }
               
            }

            return Ok(reprtedRecipes);
        }

        [HttpPost]
        public async Task<IActionResult> ReportRecipe([FromForm] long userId, [FromForm] long recipeId)
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
            var reportedByOthers = _context.ReportedRecipes.FirstOrDefault(rr => rr.RecipeId == recipeId);
            if (reportedRecipe != null)
            {
                return Ok("Already reported");
            }
            else
            {
                if(reportedByOthers != null)
                {
                    return Ok("Recipe Reported");
                }
                else
                {
                    reportedRecipe = new ReportedRecipe
                    {
                        UserId = userId,
                        RecipeId = recipeId
                    };
                    _context.ReportedRecipes.Add(reportedRecipe);
                    _context.SaveChanges();
                }
              
            }



           

            return Ok("Recipe Reported");
        }


        [HttpDelete("{recipeId}")]
        public async Task<IActionResult> AllowReportedRecipe(long recipeId)
        {
            var recipe = await _context.ReportedRecipes.FirstOrDefaultAsync(c => c.RecipeId == recipeId);
            if (recipe == null)
            {
                return BadRequest("Recipe does not exist");
            }

            _context.ReportedRecipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
