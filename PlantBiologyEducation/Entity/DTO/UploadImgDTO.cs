using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

public class UploadImgDTO   
{
    [Required]
    [FromForm(Name = "image")]
    public IFormFile Image { get; set; }
}
