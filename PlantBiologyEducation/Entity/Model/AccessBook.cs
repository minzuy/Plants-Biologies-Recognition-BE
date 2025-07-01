using Plant_BiologyEducation.Entity.Model;

namespace PlantBiologyEducation.Entity.Model
{
    public class AccessBook
    {
        public Guid AccessBook_Id { get; set; }
        public User User { get; set; }
        public Guid User_Id { get; set; }
        public Book Book { get; set; }
        public Guid Book_Id { get; set; }
        public int VisitedNumber { get; set; }
        public DateTime AccessedAt { get; set; } = DateTime.UtcNow;

    }
}
