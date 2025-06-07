namespace Plant_BiologyEducation.Entity.Model
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; } 
        public string Answer { get; set; } 
        public string TestId { get; set; } = string.Empty;
        public Test Test { get; set; } 
    }
}
