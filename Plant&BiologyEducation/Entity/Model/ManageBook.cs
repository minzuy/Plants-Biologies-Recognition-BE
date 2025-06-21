namespace Plant_BiologyEducation.Entity.Model
{
    public class ManageBook
    {
        public User User { get; set; }
        public Guid User_Id { get; set; }
        public Book Book { get; set; }
        public Guid Book_Id { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
