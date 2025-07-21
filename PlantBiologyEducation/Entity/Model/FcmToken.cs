namespace PlantBiologyEducation.Entity.Model
{
    public class FcmToken
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Platform { get; set; } // e.g., "android", "ios"
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
