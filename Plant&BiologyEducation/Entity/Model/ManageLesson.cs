namespace Plant_BiologyEducation.Entity.Model
{
    public class ManageLesson
    {
        public User User { get; set; }
        public Guid User_Id { get; set; }
        public Lesson Lesson { get; set; }
        public Guid Lesson_Id { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
