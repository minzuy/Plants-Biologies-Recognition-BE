namespace Plant_BiologyEducation.Entity.DTO.Lesson
{
    public class LessonDTO
    {
        public Guid Lesson_Id { get; set; }
        public string Lesson_Title { get; set; }
        public string Content { get; set; }

        // Nếu bạn muốn trả về tên chapter (optional)
        public Guid Chapter_Id { get; set; }
    }
}
