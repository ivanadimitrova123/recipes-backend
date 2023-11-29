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
    public class UserGradesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserGradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserGrades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGrades>>> GetUserGrades()
        {
          if (_context.UserGrades == null)
          {
              return NotFound();
          }
            return await _context.UserGrades.ToListAsync();
        }

        // GET: api/UserGrades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGrades>> GetUserGrades(int id)
        {
          if (_context.UserGrades == null)
          {
              return NotFound();
          }
            var userGrades = await _context.UserGrades.FindAsync(id);

            if (userGrades == null)
            {
                return NotFound();
            }

            return userGrades;
        }

        // PUT: api/UserGrades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserGrades(int id, UserGrades userGrades)
        {
            if (id != userGrades.Id)
            {
                return BadRequest();
            }

            _context.Entry(userGrades).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserGradesExists(id))
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

        // POST: api/UserGrades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserGrades>> PostUserGrades(UserGrades userGrades)
        {
          if (_context.UserGrades == null)
          {
              return Problem("Entity set 'ApplicationDbContext.UserGrades'  is null.");
          }
            _context.UserGrades.Add(userGrades);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserGrades", new { id = userGrades.Id }, userGrades);
        }

        // DELETE: api/UserGrades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserGrades(int id)
        {
            if (_context.UserGrades == null)
            {
                return NotFound();
            }
            var userGrades = await _context.UserGrades.FindAsync(id);
            if (userGrades == null)
            {
                return NotFound();
            }

            _context.UserGrades.Remove(userGrades);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserGradesExists(int id)
        {
            return (_context.UserGrades?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
