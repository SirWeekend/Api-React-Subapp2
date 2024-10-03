using System.Security.Cryptography;
using System.Text;

namespace Eksamen2024.Helpers
{
    public class PasswordHelper
    {
        public static bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
        public static String HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}