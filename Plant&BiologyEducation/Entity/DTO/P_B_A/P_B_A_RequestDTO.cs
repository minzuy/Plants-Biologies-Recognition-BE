namespace Plant_BiologyEducation.Entity.DTO.P_B_A
{
    public class P_B_A_RequestDTO
    {
        public string CommonName { get; set; }
        public string ScientificName { get; set; }
        public string SpecieType { get; set; } // Ví dụ: Animal, Plant
        public string Description { get; set; }
        public string Habitat { get; set; }
        public string ImageUrl { get; set; }
        public bool IsExtinct { get; set; }
        public DateTime? DiscoveredAt { get; set; }
        public int? AverageLifeSpan { get; set; }

        public Guid Lesson_Id { get; set; }
    }
}
