using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly IVideoFacade _videoFacade;
        private readonly IUserRepository _userRepository;
        public UserFacade(IVideoFacade videoFacade, IUserRepository userRepository)
        {
            _videoFacade = videoFacade;
            _userRepository = userRepository;
        }

        public async Task CountUserTagInteraction(UserTagInteraction interaction) 
            => await _videoFacade.CountUserVideoInteraction(interaction);

        public async Task RegisterVideoLike(Like like)
            => await _videoFacade.RegisterVideoLike(like);

        public async Task<UserDao> CreateUser(UserDao user)
            => await _userRepository.CreateUser(user);

        public async Task<UserDao> GetUser(string email)
            => await _userRepository.GetUser(email);

        public async Task<UserDao> GetUserByVideoBlobId(string blobId)
            => await _userRepository.GetUserByVideoBlobId(blobId);

        public async Task<bool> UserExists(string email) 
            => await _userRepository.UserExists(email);

        public Task CreateTokensOnUser(string email, string refreshToken)
            => _userRepository.CreateTokensOnUser(email, refreshToken);

        public async Task<UserDao> ValidateRefreshToken(string refreshToken)
            => await _userRepository.ValidateRefreshToken(refreshToken);

        public List<UserTagInteractionDao> GetUserTagInteractions()
            => _userRepository.GetUserTagInteractions();

        public Task RemoveRefreshToken(string refreshToken)
            => _userRepository.RemoveRefreshToken(refreshToken);
    }
}
