using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO.User
{
    public class UserRequestDTO
    {
        [Required(ErrorMessage = "Account is required")]
        [StringLength(50, ErrorMessage = "Account cannot exceed 50 characters")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Student)$", ErrorMessage = "Role must be Student only")]
        //[RegularExpression( ErrorMessage = "Role must be Teacher or Student")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; }

        public bool IsActive { get; set; } = false;
    }
}
