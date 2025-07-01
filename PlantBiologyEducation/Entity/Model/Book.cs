namespace Plant_BiologyEducation.Entity.Model
{
    public class Book
    {
        public Guid Book_Id { get; set; }
        public string Book_Title { get; set; }
        public string Cover_img { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string? RejectionReason { get; set; } // Lý do từ chối nếu có
        public ICollection<Chapter> Chapters { get; set; }
    }
}
