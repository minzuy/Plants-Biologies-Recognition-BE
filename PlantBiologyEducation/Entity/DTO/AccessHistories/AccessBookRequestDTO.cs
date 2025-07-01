namespace PlantBiologyEducation.Entity.DTO.AccessHistories
{
    public class AccessBookRequestDTO
    {
        public Guid User_Id { get; set; }
        public Guid Book_Id { get; set; }
        public DateTime AccessedAt { get; set; } = DateTime.UtcNow;
    }
}
