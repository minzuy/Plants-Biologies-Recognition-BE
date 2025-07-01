namespace PlantBiologyEducation.Entity.DTO.Chapter
{
    public class ChapterStatusUpdate
    {
        public string Status { get; set; } // "Approved" | "Rejected"
        public string? RejectionReason { get; set; }
    }
}
