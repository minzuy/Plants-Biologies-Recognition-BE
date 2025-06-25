namespace Plant_BiologyEducation.Entity.Model
{
    public class User
    {
        public Guid User_Id { get; set; }
        public string  Account { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string FullName { get; set; }

        public ICollection<ManageBook> ManagedBooks { get; set; }
        public ICollection<ManageChapter> ManagedChapters { get; set; }
        public ICollection<ManageLesson> ManagedLessons { get; set; }

    }
}
