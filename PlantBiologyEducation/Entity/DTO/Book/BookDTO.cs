using Plant_BiologyEducation.Entity.DTO.Chapter;

namespace Plant_BiologyEducation.Entity.DTO.Book
{
    public class BookDTO
    {
        public Guid Book_Id { get; set; }
        public string Book_Title { get; set; }
        public string Cover_img { get; set; }
        public List<ChapterDTO> Chapters { get; set; }
    }
}
