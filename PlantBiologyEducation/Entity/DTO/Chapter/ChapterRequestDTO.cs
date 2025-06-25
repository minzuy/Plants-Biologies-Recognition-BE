
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

        public List<LessonRequestDTO> Lessons { get; set; }
    }
}
