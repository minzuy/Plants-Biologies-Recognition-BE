 namespace Plant_BiologyEducation.Entity.Model
{
    public class Lesson
    {
        public Guid Lesson_Id { get; set; }
        public string Lesson_Title { get; set; }
        public string Content { get; set; }

        public Chapter Chapter { get; set; }
        public Guid Chapter_Id { get; set; }

        public ICollection<ManageLesson> ManagedBy { get; set; }

        public ICollection<Plant_Biology_Animals> RelatedSpecies { get; set; } // Thêm dòng này

    }
}
