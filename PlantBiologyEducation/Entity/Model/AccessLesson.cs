using Plant_BiologyEducation.Entity.Model;

namespace PlantBiologyEducation.Entity.Model
{
    public class AccessLesson
    {
        public Guid AccessLesson_Id { get; set; }
        public User User { get; set; }
        public Guid User_Id { get; set; }
        public Lesson Lesson  { get; set; }
        public Guid Lesson_Id { get; set; }
        public int VisitedNumber { get; set; }
        public DateTime AccessedAt { get; set; } = DateTime.UtcNow;
    }
}
