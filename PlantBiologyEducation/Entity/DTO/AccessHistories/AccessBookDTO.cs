namespace Plant_BiologyEducation.Entity.DTO.AccessHistories
{
    public class AccessBookDTO
    {
        public Guid User_Id { get; set; }
        public Guid Book_Id { get; set; }
        public int VisitedNumber { get; set; }
        public DateTime AccessedAt { get; set; } 

    }
}
