using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.DTO
{
    public class SubmitTestDTO
    {
            public Guid UserId { get; set; }     // lấy từ token hoặc client
            public string TestId { get; set; } = string.Empty;
            public double Result { get; set; }   // điểm số sau khi chấm

    }
}
