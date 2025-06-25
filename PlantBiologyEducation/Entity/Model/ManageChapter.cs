namespace Plant_BiologyEducation.Entity.Model
{
    public class ManageChapter
    {
        public User User { get; set; }
        public Guid User_Id { get; set; }
        public Chapter Chapter { get; set; }
        public Guid Chapter_Id { get; set; } 
        public DateTime UpdatedDate { get; set; }
    }
}
