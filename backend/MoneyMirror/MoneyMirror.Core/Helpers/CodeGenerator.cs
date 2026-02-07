using System;
using System.Security.Cryptography;

namespace MoneyMirror.Core.Helpers
{
    /// <summary>
    /// Helper class for generating secure 6-digit confirmation codes.
    /// These codes are used for email confirmation, password reset, and email change.
    /// </summary>
    public static class CodeGenerator
    {
        /// <summary>
        /// Generates a secure random 6-digit numeric code.
        /// Uses cryptographically secure random number generator.
        /// </summary>
        /// <returns>6-digit string, e.g., "483920"</returns>
        public static string Generate6DigitCode()
        {
            // Use RandomNumberGenerator for secure random numbers
            // (more secure than Random class)
            using (var rng = RandomNumberGenerator.Create())
            {
                // Generate a random number between 0 and 999999
                byte[] randomBytes = new byte[4]; // 4 bytes = 32 bits
                rng.GetBytes(randomBytes);

                // Convert bytes to integer
                int randomNumber = BitConverter.ToInt32(randomBytes, 0);

                // Make it positive and limit to 6 digits (0-999999)
                randomNumber = Math.Abs(randomNumber) % 1000000;

                // Format as 6-digit string (pad with zeros if needed)
                // Example: 123 becomes "000123"
                return randomNumber.ToString("D6");
            }
        }

        /// <summary>
        /// Calculates expiration time for a code.
        /// Codes expire after 15 minutes by default.
        /// </summary>
        /// <param name="expirationMinutes">Number of minutes until expiration (default: 15)</param>
        /// <returns>DateTime when code expires</returns>
        public static DateTime GetCodeExpiration(int expirationMinutes = 15)
        {
            return DateTime.UtcNow.AddMinutes(expirationMinutes);
        }

        /// <summary>
        /// Checks if a code has expired.
        /// </summary>
        /// <param name="expiryDateTime">When the code expires</param>
        /// <returns>True if expired, False if still valid</returns>
        public static bool IsCodeExpired(DateTime? expiryDateTime)
        {
            if (expiryDateTime == null)
                return true; // No expiry set = expired

            return expiryDateTime < DateTime.UtcNow;
        }
    }
}