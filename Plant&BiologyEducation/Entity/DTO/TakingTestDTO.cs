namespace Plant_BiologyEducation.Entity.DTO
{
    public class TakingTestDTO
    {
        public Guid UserId { get; set; }
        public string FullName  { get; set; }
        public string TestId { get; set; }
        public DateTime TakingDate { get; set; }
        public double Result { get; set; }
    }
}
