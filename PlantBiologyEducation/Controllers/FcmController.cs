using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using PlantBiologyEducation.Entity.DTO.Noti;
using PlantBiologyEducation.Service;

namespace PlantBiologyEducation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FcmController : ControllerBase
    {
        private readonly FcmTokenStore _tokenStore;
        private readonly ILogger<FcmController> _logger;

        public FcmController(FcmTokenStore tokenStore, ILogger<FcmController> logger)
        {
            _tokenStore = tokenStore;
            _logger = logger;
        }

        public class RegisterTokenRequest
        {
            public string UserId { get; set; }
            public string FcmToken { get; set; }
            public string Platform { get; set; }
        }


        [HttpPost("register-token")]
        public IActionResult RegisterToken([FromBody] RegisterTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.FcmToken) || string.IsNullOrEmpty(request.Platform))
            {
                _logger.LogWarning("RegisterToken failed: Missing UserId, FcmToken, or Platform.");
                return BadRequest("Missing UserId, FcmToken, or Platform.");
            }

            _tokenStore.SaveToken(request.UserId, request.FcmToken, request.Platform);
            return Ok(new { message = "FCM token registered successfully." });
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
        {
            if (string.IsNullOrEmpty(request.TargetUserId) || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
            {
                _logger.LogWarning("SendNotification failed: Missing TargetUserId, Title, or Body.");
                return BadRequest("Missing TargetUserId, Title, or Body.");
            }

            string targetFcmToken = _tokenStore.GetTokenByUserId(request.TargetUserId);

            if (string.IsNullOrEmpty(targetFcmToken))
            {
                _logger.LogWarning($"No FCM token found for UserId: {request.TargetUserId}. Notification not sent.");
                return NotFound($"No FCM token found for UserId: {request.TargetUserId}.");
            }

            var message = new Message()
            {
                Notification = new Notification()
                {
                    Title = request.Title,
                    Body = request.Body,
                },
                Token = targetFcmToken, // Send to a specific device token
                // Optional: add custom data payload
                Data = request.Data
            };

            try
            {
                // Send the message
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation($"Successfully sent message to {request.TargetUserId}. Response: {response}");
                return Ok(new { message = "Notification sent successfully.", firebaseResponse = response });
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, $"Error sending notification to {request.TargetUserId}. Code: {ex.MessagingErrorCode}");

                // Handle specific errors (e.g., invalid token, unregistration)
                if (ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
                {
                    _logger.LogWarning($"FCM token for {request.TargetUserId} is unregistered. Removing from store.");
                    _tokenStore.RemoveToken(request.TargetUserId);
                    return StatusCode(410, "FCM token is no longer valid or unregistered. Please re-register."); // 410 Gone
                }
                return StatusCode(500, $"Failed to send notification: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while sending notification to {request.TargetUserId}.");
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }

}
