namespace Plant_BiologyEducation.Entity.Model
{
    public class AccessBookHistory
    {
        public Guid User_Id { get; set; }
        public User User { get; set; }

        public Guid Book_Id { get; set; }
        public Book Book { get; set; }
        public int Visited_Number { get; set; }
    }
}
