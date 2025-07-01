using Plant_BiologyEducation.Entity.Model;

namespace PlantBiologyEducation.Entity.Model
{
    public class AccessBiology
    {
        public Guid AccessBiology_Id { get; set; }
        public User User { get; set; }
        public Guid User_Id { get; set; }
        public Plant_Biology_Animals Oject { get; set; }
        public Guid Oject_Id { get; set; }
        public int VisitedNumber { get; set; }
        public DateTime AccessedAt { get; set; } = DateTime.UtcNow;
    }
}
