namespace Plant_BiologyEducation.Entity.Model
{
    public class TakingTest
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string TestId { get; set; } = string.Empty;
        public Test Test { get; set; } 

        public DateTime TakingDate { get; set; }
        public double Result { get; set; } 
    }
}
