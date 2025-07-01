using Plant_BiologyEducation.Entity.DTO.Chapter;

namespace Plant_BiologyEducation.Entity.DTO.Book
{
    public class BookDTO
    {
        public Guid Book_Id { get; set; }
        public string Book_Title { get; set; }
        public string Cover_img { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string? RejectionReason { get; set; } // Lý do từ chối nếu có
        public List<ChapterDTO> Chapters { get; set; }
    }
}
