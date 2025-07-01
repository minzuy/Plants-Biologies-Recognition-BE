 namespace Plant_BiologyEducation.Entity.Model
{
    public class Lesson
    {
        public Guid Lesson_Id { get; set; }
        public string Lesson_Title { get; set; }
        public string Content { get; set; }
        public Chapter Chapter { get; set; }
        public Guid Chapter_Id { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }  // Pending, Approved, Rejected

        public string? RejectionReason { get; set; } // Lý do từ chối nếu có
        public ICollection<Plant_Biology_Animals> RelatedSpecies { get; set; } 

    }
}
