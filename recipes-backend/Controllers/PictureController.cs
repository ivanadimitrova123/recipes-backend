using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using recipes_backend.Models;

namespace recipes_backend.Controllers;


[Route("api/image")]
[ApiController]

public class PictureController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PictureController(ApplicationDbContext context)
    {
        _context = context;
    }
    
   [HttpPost]
   [Authorize]
   public async Task<IActionResult> UploadImage(IFormFile file)
   {
       if (file == null || file.Length == 0)
       {
           return BadRequest(new { error = "Invalid file" });
       }

       // Get the currently logged-in user
       var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
       if (userIdClaim == null)
       {
           return Unauthorized(new { error = "User not authenticated" });
       }

       if (long.TryParse(userIdClaim.Value, out long userId))
       {
           // Find the user by ID
           var user = _context.Users.FirstOrDefault(u => u.Id == userId);

           if (user == null)
           {
               return NotFound(new { error = "User not found" });
           }

           using (var memoryStream = new MemoryStream())
           {
               await file.CopyToAsync(memoryStream);
               var image = new Picture
               {
                   FileName = file.FileName,
                   ImageData = memoryStream.ToArray(),
                   ContentType = file.ContentType
               };

               _context.Pictures.Add(image);
               await _context.SaveChangesAsync();
               // Set the user's ProfilePictureId to the ID of the uploaded Picture
               user.ProfilePictureId = image.Id;
               await _context.SaveChangesAsync();
               // Construct the image URL based on your server's URL and the image's ID
               string imageUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{image.Id}";

               // Return the image URL in the response
               return Ok(new { imageUrl });
           }
       }
       return NotFound(new { error = "User not found or conversion failed" });
   }

    
    
    [HttpGet("{id}")]
    public IActionResult GetImage(long id)
    {
        var image = _context.Pictures.Find(id);

        if (image == null)
        {
            return NotFound("Image not found");
        }
        return File(image.ImageData, image.ContentType);

        // return File(image.ImageData, "image/png"); // Adjust the content type as needed (e.g., image/png)
    }
}