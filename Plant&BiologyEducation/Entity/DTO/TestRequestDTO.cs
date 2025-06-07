using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO
{
    public class TestRequestDTO
    {
        [Required(ErrorMessage = "CreatorId là bắt buộc.")]
        public Guid CreatorId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow; // Default value

    }
}
