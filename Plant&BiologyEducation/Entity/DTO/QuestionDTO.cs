namespace Plant_BiologyEducation.Entity.DTO
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }

        // Thêm TestId để xác định nó thuộc bài kiểm tra nào
        public int TestId { get; set; }
    }
}
