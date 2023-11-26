﻿using System.Text;
using TikTakServer.Handlers;
using TikTakServer.Models;
using TikTakServer.Models.Business;
using TikTakServer.Repositories;

namespace TikTakServer.ApplicationServices
{
    public class AuthenticationService
    {
        private readonly IUserRepository userRepository;
        private readonly JwtHandler jwtHandler;
        private readonly GoogleAuthService googleAuth;
        private readonly UserRequestAndClaims _requestAndClaims;

        public AuthenticationService(IUserRepository userRepository, JwtHandler jwtHandler, GoogleAuthService googleAuthService, UserRequestAndClaims requestClaims)
        {
            this.userRepository = userRepository;
            this.jwtHandler = jwtHandler;
            this.googleAuth = googleAuthService;
            this._requestAndClaims = requestClaims;
        }

        public async Task<User> RefreshAccessToken(string refreshToken)
        {
            var user = await userRepository.ValidateRefreshToken(refreshToken);
            if (user == null && _requestAndClaims.UserId != user.Id.ToString())
            {
                throw new Exception("Token could not be matched with a user, try again.");
            }
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl, user.Country);
            return new User(user, accessToken, refreshToken);
        }

        private async Task<User> CreateUser(GoogleInfoModel infoModel,string name, string imgUrl, string country)
        {
            var refreshToken = jwtHandler.CreateRefreshToken();
            var newUser = CreateUserDao(infoModel, name, imgUrl, refreshToken, country);
            var user = await userRepository.CreateUser(newUser);
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl, country);
            return new User(user, accessToken, refreshToken);
        }

        public async Task<User> Login(string googleAccessToken, string name, string imgUrl, double longi, double lati)
        {
            var googleUser = await googleAuth.VerifyTokenAsync(googleAccessToken);
            var countryName = await googleAuth.GetCountryOfLocation(longi, lati);
            var userExists = await userRepository.UserExists(googleUser.Email);
            if (!userExists)
            {
                return await CreateUser(googleUser, name, imgUrl, countryName);
            }
            var user = await userRepository.GetUser(googleUser.Email);
            var refreshToken = jwtHandler.CreateRefreshToken();
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl, countryName);
            await userRepository.CreateTokensOnUser(user.Email, refreshToken);
            return new User(user, accessToken, refreshToken);
        }

        private UserDao CreateUserDao(GoogleInfoModel infoModel, string name, string imgUrl, string refreshToken, string country)
        {
            return new UserDao()
            {
                FullName = name,
                Email = infoModel.Email,
                ImageUrl = imgUrl,
                Country = country,
                DateOfBirth = DateTime.Now.AddYears(-14),
                Tokens = new List<UserTokenDao>()
                {
                    new UserTokenDao()
                    {
                        RefreshToken = refreshToken,
                        RefreshTokenExpiry = DateTime.Now.AddDays(200)
                    }
                }
            };
        }
    }
}
