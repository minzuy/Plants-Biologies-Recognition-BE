﻿namespace Plant_BiologyEducation.Entity.DTO.P_B_A
{
    public class P_B_A_DTO
    {
        public Guid Id { get; set; }
        public string CommonName { get; set; }
        public string ScientificName { get; set; }
        public string SpecieType { get; set; } // Ví dụ: Animal, Plant
        public string Description { get; set; }
        public string Habitat { get; set; }
        public string ImageUrl { get; set; }
        public bool IsExtinct { get; set; }
        public string DiscoveredAt { get; set; }
        public string AverageLifeSpan { get; set; }

        public bool IsActive { get; set; }
        public string Status { get; set; }  // Pending, Approved, Rejected

        public string? RejectionReason { get; set; } // Lý do từ chối nếu có
        public Guid Lesson_Id { get; set; }

    }
}
