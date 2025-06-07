using System;

namespace Plant_BiologyEducation.Service
{
    public class TestService
    {
        private static readonly Random _random = new Random();
        public static string GenerateRandomTestId()
        {
            return _random.Next(100000, 999999).ToString();
        }
    }
}
