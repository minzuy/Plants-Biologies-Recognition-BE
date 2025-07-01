namespace PlantBiologyEducation.Entity.DTO.Book
{
    public class BookStatusUpdateDTO
    {
        public string Status { get; set; } // "Approved" | "Rejected"
        public string? RejectionReason { get; set; }
    }
}
