
using Plant_BiologyEducation.Entity.DTO.Lesson;
using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO.Chapter
{
    public class ChapterRequestDTO
    {
        [Required(ErrorMessage = "Book_Id is required.")]
        public Guid Book_Id { get; set; }

        [Required]
        public string Chapter_Title { get; set; }

        public bool IsActive { get; set; }
        public string Status { get; set; }  // Pending, Approved, Rejected

        public string? RejectionReason { get; set; } // Lý do từ chối nếu có

    }
}
