namespace Plant_BiologyEducation.Entity.DTO.AccessHistories
{
    public class AccessLessonDTO
    {
        public Guid User_Id { get; set; }
        public Guid Lesson_Id { get; set; }
        public int VisitedNumber { get; set; }
        public DateTime AccessedAt { get; set; }
    }
}
