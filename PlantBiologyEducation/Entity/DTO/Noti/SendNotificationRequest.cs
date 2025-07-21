namespace PlantBiologyEducation.Entity.DTO.Noti
{
    public class SendNotificationRequest
    {
        public string TargetUserId { get; set; } // The user to send the notification to
        public string Title { get; set; }
        public string Body { get; set; }
        // Optional: you can add a data payload here
        public Dictionary<string, string> Data { get; set; }
    }
}
