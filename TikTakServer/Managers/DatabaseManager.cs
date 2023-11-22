using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TikTakServer.Database;

namespace TikTakServer.Managers
{
    public class DatabaseManager
    {
        public string GenerateSalt()
        {
            using (var randomGenerator = new RNGCryptoServiceProvider())
            {
                byte[] buff = new byte[8];
                randomGenerator.GetBytes(buff);
                return Convert.ToBase64String(buff);
            }
        }
        public string GenerateHashedSalt(string passwrd, string salt)
        {
            byte[] pwdWithSalt = Encoding.ASCII.GetBytes(string.Concat(passwrd, salt));
            using (var sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(pwdWithSalt));
            }
        }

        public string HashPassword(string passwrd, string salt)
        {
            byte[] pwdWithSalt = Encoding.ASCII.GetBytes(string.Concat(passwrd, salt));
            using (var sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(pwdWithSalt));
            }
        }

        public string GetSaltFromDB(string username, TikTakContext context)
        {
            using (context)
            {
                var user = context.Users.Where(x => x.UserName.Equals(username)).FirstOrDefault();
                if (user != null)
                    return user.PasswordSalt;

                else return null;
            }
        }

        public bool ValidateUsername(string username, TikTakContext context)
        {
            using (context)
            {
                var user = context.Users.Where(x => x.UserName.Equals(username)).FirstOrDefault();
                if (user == null)
                    return false;

                if (user.UserName.Equals(username))
                    return true;

                return false;
            }
        }
        public bool ValidatePassword(string password, string salt, string HashedPassword)
        {
            byte[] pwdWithSaltFromDB = Encoding.ASCII.GetBytes(string.Concat(password, salt));
            using (var sha256 = SHA256.Create())
            {
                if (HashedPassword == Convert.ToBase64String(sha256.ComputeHash(pwdWithSaltFromDB)))
                    return true;

                else
                    return false;
            }
        }


    }
}
