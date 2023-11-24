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
            if (user == null || _requestAndClaims.UserId != user.Id.ToString())
            {
                throw new Exception("Token could not be matched with a user, try again.");
            }
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl);
            return new User(user, accessToken);
        }

        private async Task<User> CreateUser(GoogleInfoModel infoModel,string name, string imgUrl)
        {
            var refreshToken = jwtHandler.CreateRefreshToken();
            var newUser = CreateNewUser(infoModel, name, imgUrl, refreshToken);
            var user = await userRepository.CreateUser(newUser);
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl);
            return new User(user, accessToken);
        }

        public async Task<User> Login(string googleAccessToken, string name, string imgUrl)
        {
            var googleUser = await googleAuth.VerifyTokenAsync(googleAccessToken);
            var userExists = await userRepository.UserExists(googleUser.Email);
            if (!userExists)
            {
                return await CreateUser(googleUser, name, imgUrl);
            }
            var user = await userRepository.GetUser(googleUser.Email);
            var refreshToken = jwtHandler.CreateRefreshToken();
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl);
            await userRepository.CreateTokensOnUser(user.Email, refreshToken);
            return new User(user, accessToken);
        }


        private UserDao CreateNewUser(GoogleInfoModel infoModel, string name, string imgUrl, string refreshToken)
        {
            return new UserDao()
            {
                FullName = name,
                Email = infoModel.Email,
                ImageUrl = imgUrl,
                DateOfBirth = DateTime.Now.AddYears(-14),
                Token = new UserTokenDao()
                {
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = DateTime.Now.AddDays(200)
                }
            };
        }
    }
}
