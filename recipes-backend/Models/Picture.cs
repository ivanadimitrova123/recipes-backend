using System.ComponentModel.DataAnnotations;

namespace recipes_backend.Models;

public class Picture
{
    [Key]
    public long Id { get; set; }
    public byte[] ImageData { get; set; } // Byte array to store image data
    public string FileName { get; set; } // Store the file name
    public string ContentType { get; set; }
}