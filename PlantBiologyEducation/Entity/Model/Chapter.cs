namespace Plant_BiologyEducation.Entity.Model
{
    public class Chapter
    {
        public Guid Chapter_Id { get; set; }
        public string Chapter_Title { get; set; }
        public Book Book { get; set; }
        public Guid Book_Id { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
        public ICollection<ManageChapter> ManagedBy { get; set; }
    }
}
