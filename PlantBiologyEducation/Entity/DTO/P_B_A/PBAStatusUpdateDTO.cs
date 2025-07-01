namespace PlantBiologyEducation.Entity.DTO.P_B_A
{
    public class PBAStatusUpdateDTO
    {
        public string Status { get; set; } // "Approved" | "Rejected"
        public string? RejectionReason { get; set; }
    }
}
