using TikTakServer.Handlers;
using TikTakServer.Models.DaoModels;
using TikTakServer.Models.Business;
using TikTakServer.Facades;

namespace TikTakServer.ApplicationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserFacade userFacade;
        private readonly IJwtHandler jwtHandler;
        private readonly IGoogleAuthService googleAuth;

        public AuthenticationService(IUserFacade userFacade, IJwtHandler jwtHandler, IGoogleAuthService googleAuthService)
        {
            this.userFacade = userFacade;
            this.jwtHandler = jwtHandler;
            this.googleAuth = googleAuthService;
        }

        public async Task<User> RefreshAccessToken(string refreshToken)
        {
            var isRefreshTokenValid = await userFacade.IsRefreshTokenValid(refreshToken);
            var user = await userFacade.GetUserOnRefreshToken(refreshToken);
            if (!isRefreshTokenValid)
            {
                throw new Exception("Token could not be matched with a user, try again.");
            }
            var accessToken = jwtHandler.CreateJwtAccess(user.Email, user.ImageUrl);
            user.AccessToken = accessToken;
            return user;
        }

        private async Task<User> CreateUser(GoogleInfoModel infoModel,string name, string imgUrl)
        {
            var refreshToken = jwtHandler.CreateRefreshToken();
            var newUser = CreateUserModel(infoModel, name, imgUrl, refreshToken);
            var user = await userFacade.CreateUser(newUser);
            var accessToken = jwtHandler.CreateJwtAccess(user.Email, user.ImageUrl);
            user.AccessToken = accessToken;
            return user;
        }

        public async Task<User> Login(string googleAccessToken, string name, string imgUrl)
        {
            var googleUser = await googleAuth.VerifyToken(googleAccessToken);
            var userExists = await userFacade.UserExists(googleUser.Email);
            if (!userExists)
            {
                return await CreateUser(googleUser, name, imgUrl);
            }
            var user = await userFacade.GetUserOnRefreshToken(googleUser.Email);
            var refreshToken = jwtHandler.CreateRefreshToken();
            var accessToken = jwtHandler.CreateJwtAccess(user.Email, user.ImageUrl);
            await userFacade.CreateTokensOnUser(user.Email, refreshToken);
            user.AccessToken = accessToken;
            return user;
        }

        public async Task Logout(string refreshToken)
        {
            await userFacade.RemoveRefreshToken(refreshToken);
        }

        private User CreateUserModel(GoogleInfoModel infoModel, string name, string imgUrl, string refreshToken)
        {
            return new User()
            {
                FullName = name,
                Email = infoModel.Email,
                ImageUrl = imgUrl,
                DateOfBirth = DateTime.Now.AddYears(-14),
                RefreshToken = refreshToken
            };
        }
    }
}
