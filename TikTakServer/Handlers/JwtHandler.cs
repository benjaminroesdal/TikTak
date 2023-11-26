using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TikTakServer.Handlers
{
    public class JwtHandler
    {
        private readonly string _secretKey;
        private readonly IConfiguration config;

        public JwtHandler(IConfiguration configuration)
        {
            _secretKey = configuration["SecretKey"];
            config = configuration;
        }

        public List<Claim> CreateClaims(int userId, string userEmail, string userImg, string countryName)
        {
            var claims = new List<Claim>
            {
                new Claim("user_id", userId.ToString()),
                new Claim("user_email", userEmail),
                new Claim("profile_img", userImg),
                new Claim("user_country", countryName)
            };
            return claims;
        }

        public string CreateJwtAccess(int userId, string userEmail, string userImg, string countryName)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var allClaims = CreateClaims(userId, userEmail, userImg, countryName);

            var token = new JwtSecurityToken(
                issuer: config["Issuer"],
                audience: config["Audience"],
                claims: allClaims,
                expires: DateTime.UtcNow.AddSeconds(80),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
