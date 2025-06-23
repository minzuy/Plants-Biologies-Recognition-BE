using Plant_BiologyEducation.Entity.DTO.Lesson;
using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO.Chapter
{
    public class ChapterDTO
    {
        public Guid Book_Id { get; set; }
        public Guid Chapter_Id { get; set; }
        public string Chapter_Title { get; set; }
        public List<LessonDTO> Lessons { get; set; }
    }
}
