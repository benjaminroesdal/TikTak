using TikTakServer.Handlers;
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

        /// <summary>
        /// Verifies validity of provided refreshToken, if valid creates a new JWT access token and returns the updated user object
        /// </summary>
        /// <param name="refreshToken">RefreshToken to verify</param>
        /// <returns>Updated user model with new JWT access token</returns>
        public async Task<User> RefreshAccessToken(string refreshToken)
        {
            var isRefreshTokenValid = await userFacade.IsRefreshTokenValid(refreshToken);
            if (!isRefreshTokenValid)
            {
                throw new Exception("Token could not be matched with a user, try again.");
            }
            var user = await userFacade.GetUserOnRefreshToken(refreshToken);
            var accessToken = jwtHandler.CreateJwtAccess(user.Email, user.ImageUrl);
            user.AccessToken = accessToken;
            return user;
        }

        /// <summary>
        /// Creates a user in database based on google infoModel and creates tokens for this user.
        /// </summary>
        /// <param name="infoModel">Model containing google tokenInfo information</param>
        /// <param name="name">Name of google user</param>
        /// <param name="imgUrl">img url path</param>
        /// <returns>User model of created user.</returns>
        private async Task<User> CreateUser(GoogleInfoModel infoModel,string name, string imgUrl)
        {
            var refreshToken = jwtHandler.CreateRefreshToken();
            var newUser = CreateUserModel(infoModel, name, imgUrl, refreshToken);
            var user = await userFacade.CreateUser(newUser);
            var accessToken = jwtHandler.CreateJwtAccess(user.Email, user.ImageUrl);
            user.AccessToken = accessToken;
            return user;
        }

        /// <summary>
        /// Verifies provided googleAccess token and creates JWT/Refresh tokens on the user.
        /// If user doesn't exist, a user is created in the database.
        /// </summary>
        /// <param name="googleAccessToken">access token from google OAuth</param>
        /// <param name="name">google user name</param>
        /// <param name="imgUrl">image Url of profile img</param>
        /// <returns>Returns UserModel containing user information and Tokens</returns>
        public async Task<User> Login(string googleAccessToken, string name, string imgUrl)
        {
            var googleUser = await googleAuth.VerifyToken(googleAccessToken);
            var userExists = await userFacade.UserExists(googleUser.Email);
            if (!userExists)
            {
                return await CreateUser(googleUser, name, imgUrl);
            }
            var user = await userFacade.GetUser(googleUser.Email);
            var refreshToken = jwtHandler.CreateRefreshToken();
            var accessToken = jwtHandler.CreateJwtAccess(user.Email, user.ImageUrl);
            await userFacade.CreateTokensOnUser(user.Email, refreshToken);
            user.RefreshToken = refreshToken;
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
