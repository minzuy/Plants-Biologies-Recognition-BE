namespace Plant_BiologyEducation.Entity.Model
{
    public class Test
    {
        public string Id { get; set; } = string.Empty; // Thay đổi từ int sang string
        public DateTime DateCreated { get; set; }

        public Guid CreatorId { get; set; }
        public User Creator { get; set; } = null!;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<TakingTest> Takings { get; set; } = new List<TakingTest>();
    }
}
