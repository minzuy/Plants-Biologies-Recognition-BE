namespace Plant_BiologyEducation.Entity.DTO
{
    public class TestDTO
    {
        public string Id { get; set; } = string.Empty; 
        public DateTime DateCreated { get; set; }

        public Guid CreatorId { get; set; }
        public string? CreatorName { get; set; }  // từ User.FullName

        public int QuestionCount { get; set; }    // tổng số câu hỏi

        public int TakingCount { get; set; }      // số lượt làm bài (nếu cần)

        public List<QuestionDTO> Questions { get; set; } = new();
    }
}
