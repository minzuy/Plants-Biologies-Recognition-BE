namespace PlantBiologyEducation.Entity.DTO.Noti
{
    public class RegisterTokenRequest
    {
        public string UserId { get; set; }
        public string FcmToken { get; set; }
        public string Platform { get; set; }
    }
}
