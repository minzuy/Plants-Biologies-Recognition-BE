using Plant_BiologyEducation.Entity.DTO.P_B_A;

namespace Plant_BiologyEducation.Entity.DTO.Lesson
{
    public class LessonDTO
    {
        public Guid Chapter_Id { get; set; }

        public Guid Lesson_Id { get; set; }
        public string Lesson_Title { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public string? RejectionReason { get; set; } // Lý do từ chối nếu có
        public List<P_B_A_DTO> Plant_Biology_Animal{ get; set; }

    }
}
