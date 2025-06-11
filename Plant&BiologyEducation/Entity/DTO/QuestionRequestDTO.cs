using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO
{
    public class QuestionRequestDTO
    {
        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc")]
        [StringLength(1000, ErrorMessage = "Nội dung câu hỏi không được vượt quá 1000 ký tự")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Đáp án là bắt buộc")]
        [StringLength(500, ErrorMessage = "Đáp án không được vượt quá 500 ký tự")]
        public string Answer { get; set; }

        [Required(ErrorMessage = "Test ID là bắt buộc")]
        public string TestId { get; set; }
    }
}
