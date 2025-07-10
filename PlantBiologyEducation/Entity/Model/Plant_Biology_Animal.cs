using Plant_BiologyEducation.Entity.Model;

public class Plant_Biology_Animals
{
    public Guid Id { get; set; }
    public string CommonName { get; set; }
    public string ScientificName { get; set; }
    public string SpecieType { get; set; } // Ví dụ: Animal, Plant
    public string Description { get; set; }
    public string Habitat { get; set; }
    public string ImageUrl { get; set; }
    public bool IsExtinct { get; set; }
    public DateTime? DiscoveredAt { get; set; }
    public string AverageLifeSpan { get; set; }

    public bool IsActive { get; set; }
    public string Status { get; set; } // Pending, Approved, Rejected
    public string? RejectionReason { get; set; } // Lý do từ chối nếu có

    // Mối quan hệ nhiều-1 với Lesson
    public Guid LessonId { get; set; }
    public Lesson Lesson { get; set; }
}
