using Plant_BiologyEducation.Entity.DTO.Chapter;
using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO.Book
{
    public class BookRequestDTO
    {
        [Required]
        public string Book_Title { get; set; }

        public string Cover_img { get; set; }

        public List<ChapterRequestDTO> Chapters { get; set; }
    }
}
