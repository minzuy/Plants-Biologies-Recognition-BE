namespace Plant_BiologyEducation.Entity.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string  Account { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string FullName { get; set; }


        public ICollection<Test> CreatedTests { get; set; } = new List<Test>();
        public ICollection<TakingTest> Takings { get; set; } = new List<TakingTest>();

    }
}
