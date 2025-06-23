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

    }
}
