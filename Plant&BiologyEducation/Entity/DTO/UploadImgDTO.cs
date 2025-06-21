using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class UploadImgDTO   
{
    [Required]
    [FromForm(Name = "file")]
    public IFormFile File { get; set; }
}
