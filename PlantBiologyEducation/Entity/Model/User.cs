namespace Plant_BiologyEducation.Entity.Model
{
    public class User
    {
        public Guid User_Id { get; set; }

        //public string? FcmToken { get; set; }
        //public string? Platform { get; set; }
        public string Email { get; set; }
        public string  Account { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string FullName { get; set; }
        public bool IsActive { get; set; }

        public string? resetToken { get; set; }

    }
}
