namespace Plant_BiologyEducation.Entity.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

        // Danh sách các bài test mà người dùng đã làm
        public ICollection<TakingTestDTO>? Takings { get; set; }

    }
}
