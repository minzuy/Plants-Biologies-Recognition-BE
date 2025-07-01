namespace PlantBiologyEducation.Entity.DTO.Lesson
{
    public class LessonStatusUpdate
    {
        public string Status { get; set; } // "Approved" | "Rejected"
        public string? RejectionReason { get; set; }
    }
}
