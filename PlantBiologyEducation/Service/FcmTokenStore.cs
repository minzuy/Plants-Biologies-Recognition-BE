
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using PlantBiologyEducation.Entity.Model;
using System.Collections.Concurrent;
namespace PlantBiologyEducation.Service
{ 
    public class FcmTokenStore
    {
            // Using ConcurrentDictionary for thread-safe in-memory storage
            // Key: UserId, Value: FcmToken object
            private readonly ConcurrentDictionary<string, FcmToken> _tokens = new ConcurrentDictionary<string, FcmToken>();

            public void SaveToken(string userId, string fcmToken, string platform)
            {
                var newToken = new FcmToken
                {
                    UserId = userId,
                    Token = fcmToken,
                    Platform = platform,
                    LastUpdated = DateTime.UtcNow
                };

                // Add or Update the token for the given userId
                _tokens.AddOrUpdate(userId, newToken, (key, existingToken) =>
                {
                    existingToken.Token = fcmToken;
                    existingToken.Platform = platform;
                    existingToken.LastUpdated = DateTime.UtcNow;
                    return existingToken;
                });

                Console.WriteLine($"Token saved/updated for UserId: {userId}, Token: {fcmToken}, Platform: {platform}");
            }

            public string GetTokenByUserId(string userId)
            {
                if (_tokens.TryGetValue(userId, out FcmToken fcmToken))
                {
                    return fcmToken.Token;
                }
                return null; // Token not found
            }

            public List<string> GetAllTokens()
            {
                return _tokens.Values.Select(t => t.Token).ToList();
            }

            public void RemoveToken(string userId)
            {
                _tokens.TryRemove(userId, out _);
                Console.WriteLine($"Token removed for UserId: {userId}");
            }
        }

    }
