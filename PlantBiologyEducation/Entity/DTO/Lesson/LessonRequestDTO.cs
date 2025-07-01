using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO.Lesson
{
    public class LessonRequestDTO
    {
        [Required(ErrorMessage = "Chapter_Id is required.")]
        public Guid Chapter_Id { get; set; }

        [Required]
        public string Lesson_Title { get; set; }

        public string Content { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }  // Pending, Approved, Rejected

        public string? RejectionReason { get; set; } // Lý do từ chối nếu có
    }
}
