namespace Plant_BiologyEducation.Entity.DTO.User
{
    public class UserDTO
    {
        public Guid User_Id { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

        public bool IsActive { get; set; }
    }
}
