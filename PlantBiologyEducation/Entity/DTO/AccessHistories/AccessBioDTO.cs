namespace PlantBiologyEducation.Entity.DTO.AccessHistories
{
    public class AccessBioDTO
    {
        public Guid User_Id { get; set; }
        public Guid Bio_Id { get; set; }
        public int VisitedNumber { get; set; }
        public DateTime AccessedAt { get; set; }
    }
}
