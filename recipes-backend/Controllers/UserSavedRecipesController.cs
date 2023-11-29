using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend;
using recipes_backend.Models;

namespace recipes_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSavedRecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserSavedRecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserSavedRecipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSavedRecipe>>> GetUserSavedRecipe()
        {
          if (_context.UserSavedRecipe == null)
          {
              return NotFound();
          }
            return await _context.UserSavedRecipe.ToListAsync();
        }

        // GET: api/UserSavedRecipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSavedRecipe>> GetUserSavedRecipe(int id)
        {
          if (_context.UserSavedRecipe == null)
          {
              return NotFound();
          }
            var userSavedRecipe = await _context.UserSavedRecipe.FindAsync(id);

            if (userSavedRecipe == null)
            {
                return NotFound();
            }

            return userSavedRecipe;
        }

        // PUT: api/UserSavedRecipes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserSavedRecipe(int id, UserSavedRecipe userSavedRecipe)
        {
            if (id != userSavedRecipe.Id)
            {
                return BadRequest();
            }

            _context.Entry(userSavedRecipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSavedRecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserSavedRecipes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserSavedRecipe>> PostUserSavedRecipe(UserSavedRecipe userSavedRecipe)
        {

          if (_context.UserSavedRecipe == null)
          {
              return Problem("Entity set 'ApplicationDbContext.UserSavedRecipe'  is null.");
          }
            _context.UserSavedRecipe.Add(userSavedRecipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserSavedRecipe", new { id = userSavedRecipe.Id }, userSavedRecipe);
        }

        // DELETE: api/UserSavedRecipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSavedRecipe(int id)
        {
            if (_context.UserSavedRecipe == null)
            {
                return NotFound();
            }
            var userSavedRecipe = await _context.UserSavedRecipe.FindAsync(id);
            if (userSavedRecipe == null)
            {
                return NotFound();
            }

            _context.UserSavedRecipe.Remove(userSavedRecipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserSavedRecipeExists(int id)
        {
            return (_context.UserSavedRecipe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
