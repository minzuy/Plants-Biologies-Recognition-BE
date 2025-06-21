namespace Plant_BiologyEducation.Entity.Model
{
    public class AccessLessonHistory
    {
        public Guid User_Id { get; set; }
        public User User { get; set; }

        public Guid Lesson_Id { get; set; }
        public Lesson Lesson { get; set; }

        public int Visited_Number { get; set; }
    }
}
