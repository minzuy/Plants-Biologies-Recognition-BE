namespace Plant_BiologyEducation.Entity.Model
{
    public class Chapter
    {
        public Guid Chapter_Id { get; set; }
        public string Chapter_Title { get; set; }
        public Book Book { get; set; }
        public Guid Book_Id { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string? RejectionReason { get; set; } // Lý do từ chối nếu có
        public ICollection<Lesson> Lessons { get; set; }
    }
}
