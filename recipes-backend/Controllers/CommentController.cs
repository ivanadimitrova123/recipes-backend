using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models;

namespace recipes_backend.Controllers
{
    [Route("api/comments")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetCommentsForRecipe(long id)
        {
            var recipe = _context.Recipes
            .Include(r => r.Picture)
            .Include(r => r.User)
            .FirstOrDefault(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound("Recipe not found");
            }

            List<Comment> comments = await _context.Comments
                .Where(c => c.RecipeId == id)
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Children)
                .Include(c => c.User)
                .ToListAsync();

            List<object> editedComments = new List<object>();
            foreach (var comment in comments) {
                string userImage = "";
                if(comment.User.ProfilePictureId != null)
                {
                    userImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{comment.User.ProfilePictureId}";
                }
                editedComments.Add(new
                {
                    comment.CommentId,
                    comment.User.Username,
                    userImage,
                    comment.Content,
                  
                });
            }
          

            return Ok(editedComments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromForm] Comment model)
        {
            if (ModelState.IsValid)
            {
               await _context.Comments.AddAsync(model);
               await _context.SaveChangesAsync();
               return Ok();
            }
            return BadRequest("Error with creating comment");
        }
        
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comments = await _context.Comments.Include(x => x.Children).ToListAsync();

            var flatten = Flatten(comments.Where(c => c.CommentId == commentId));

            _context.Comments.RemoveRange(flatten);

            await _context.SaveChangesAsync();
            return Ok();
        }
        IEnumerable<Comment> Flatten(IEnumerable<Comment> comments) =>
       comments.SelectMany(x => Flatten(x.Children)).Concat(comments);

    }
}
