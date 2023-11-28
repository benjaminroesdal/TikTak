using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly IVideoFacade _videoFacade;
        private readonly IUserFacade _userFacade;
        public UserFacade(IVideoFacade videoFacade, IUserFacade userFacade)
        {
            _videoFacade = videoFacade;
            _userFacade = userFacade;
        }

        public async Task CountUserTagInteraction(UserTagInteraction interaction) 
            => await _videoFacade.CountUserVideoInteraction(interaction);

        public async Task RegisterVideoLike(Like like)
            => await _videoFacade.RegisterVideoLike(like);

        public async Task<UserDao> CreateUser(UserDao user)
            => await _userFacade.CreateUser(user);

        public async Task<UserDao> GetUser(string email)
            => await _userFacade.GetUser(email);

        public async Task<UserDao> GetUserByVideoBlobId(string blobId)
            => await _userFacade.GetUserByVideoBlobId(blobId);

        public async Task<bool> UserExists(string email) 
            => await _userFacade.UserExists(email);

        public Task CreateTokensOnUser(string email, string refreshToken)
            => _userFacade.CreateTokensOnUser(email, refreshToken);

        public async Task<UserDao> ValidateRefreshToken(string refreshToken)
            => await _userFacade.ValidateRefreshToken(refreshToken);

        public List<UserTagInteractionDao> GetUserTagInteractions()
            => _userFacade.GetUserTagInteractions();
    }
}
