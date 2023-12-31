﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TikTakServer.Handlers
{
    public class JwtHandler : IJwtHandler
    {
        private readonly string _secretKey;
        private readonly IConfiguration config;

        public JwtHandler(IConfiguration configuration)
        {
            _secretKey = configuration["SecretKey"] ?? "";
            config = configuration;
        }

        private List<Claim> CreateClaims(string userEmail, string userImg)
        {
            var claims = new List<Claim>
            {
                new Claim("user_email", userEmail),
                new Claim("profile_img", userImg),
            };
            return claims;
        }

        /// <summary>
        /// Creates a signed JWT with claims and configurations.
        /// </summary>
        /// <param name="userEmail">Email to put into claims</param>
        /// <param name="userImg">UserImg to put into claims</param>
        /// <returns>the final JWT</returns>
        public string CreateJwtAccess(string userEmail, string userImg)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var allClaims = CreateClaims(userEmail, userImg);

            var token = new JwtSecurityToken(
                issuer: config["Issuer"],
                audience: config["Audience"],
                claims: allClaims,
                expires: DateTime.UtcNow.AddSeconds(80),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Creates a random string for use as refreshToken.
        /// </summary>
        /// <returns>Random string</returns>
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
