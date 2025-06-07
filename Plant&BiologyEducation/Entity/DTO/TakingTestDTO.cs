namespace Plant_BiologyEducation.Entity.DTO
{
    public class TakingTestDTO
    {
        public int TestId { get; set; }
        public string? TestTitle { get; set; } // Nếu muốn hiển thị tên bài test
        public DateTime TakingDate { get; set; }
        public double Result { get; set; }
    }
}
