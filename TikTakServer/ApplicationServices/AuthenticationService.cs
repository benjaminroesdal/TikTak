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
        private readonly UserRequestAndClaims _requestAndClaims;

        public AuthenticationService(IUserFacade userFacade, IJwtHandler jwtHandler, IGoogleAuthService googleAuthService, UserRequestAndClaims requestClaims)
        {
            this.userFacade = userFacade;
            this.jwtHandler = jwtHandler;
            this.googleAuth = googleAuthService;
            this._requestAndClaims = requestClaims;
        }

        public async Task<User> RefreshAccessToken(string refreshToken)
        {
            var user = await userFacade.ValidateRefreshToken(refreshToken);
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
            var user = await userFacade.CreateUser(newUser);
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl, country);
            return new User(user, accessToken, refreshToken);
        }

        public async Task<User> Login(string googleAccessToken, string name, string imgUrl, double longi, double lati)
        {
            var googleUser = await googleAuth.VerifyToken(googleAccessToken);
            var countryName = await googleAuth.GetCountryOfLocation(longi, lati);
            var userExists = await userFacade.UserExists(googleUser.Email);
            if (!userExists)
            {
                return await CreateUser(googleUser, name, imgUrl, countryName);
            }
            var user = await userFacade.GetUser(googleUser.Email);
            var refreshToken = jwtHandler.CreateRefreshToken();
            var accessToken = jwtHandler.CreateJwtAccess(user.Id, user.Email, user.ImageUrl, countryName);
            await userFacade.CreateTokensOnUser(user.Email, refreshToken);
            return new User(user, accessToken, refreshToken);
        }

        public async Task Logout(string refreshToken)
        {
            await userFacade.RemoveRefreshToken(refreshToken);
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
