namespace Plant_BiologyEducation.Entity.Model
{
    public class Book
    {
        public Guid Book_Id { get; set; }
        public string Book_Title { get; set; }
        public string Cover_img { get; set; }
        public ICollection<Chapter> Chapters { get; set; }
        public ICollection<ManageBook> ManagedBy { get; set; }
        public ICollection<AccessBookHistory> AccessHistories { get; set; }
    }
}
