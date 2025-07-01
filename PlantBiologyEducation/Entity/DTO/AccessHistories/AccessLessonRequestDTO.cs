namespace PlantBiologyEducation.Entity.DTO.AccessHistories
{
    public class AccessLessonRequestDTO
    {
        public Guid User_Id { get; set; }
        public Guid Lesson_Id { get; set; }
        public DateTime AccessedAt { get; set; } = DateTime.UtcNow;
    }
}
