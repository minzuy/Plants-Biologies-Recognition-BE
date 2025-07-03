using Plant_BiologyEducation.Entity.DTO.Lesson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Plant_BiologyEducation.Entity.DTO.Chapter
{
    public class ChapterDTO
    {
        public Guid Chapter_Id { get; set; }
        public string Chapter_Title { get; set; }

        [JsonPropertyOrder(1)]
        public bool IsActive { get; set; }

        [JsonPropertyOrder(2)]
        public string? Status { get; set; }

        [JsonPropertyOrder(3)]
        public string? RejectionReason { get; set; }

        [JsonPropertyOrder(4)]

        public Guid Book_Id { get; set; }

        [JsonPropertyOrder(5)]
        public List<LessonDTO> Lessons { get; set; }
    }
}
