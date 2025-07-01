namespace PlantBiologyEducation.Entity.DTO.AccessHistories
{
    public class AccessBioRequestDTO
    {
        public Guid User_Id { get; set; }
        public Guid Bio_Id { get; set; }
        public DateTime AccessedAt { get; set; }
    }
}
