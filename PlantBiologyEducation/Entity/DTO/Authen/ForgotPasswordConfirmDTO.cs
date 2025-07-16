namespace PlantBiologyEducation.Entity.DTO.Authen
{
    public class ForgotPasswordConfirmDTO
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public string NewPassword { get; set; }
    }
}
