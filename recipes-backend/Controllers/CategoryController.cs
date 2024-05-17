using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using recipes_backend.Models;

namespace recipes_backend.Controllers;

[Route("api/category")]
[ApiController]
public class CategoryController: ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("addCategory")]
    public IActionResult AddCategory([FromBody] Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return Ok("Category added successfully.");
        }

        return BadRequest(ModelState);
    }
}