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
        {
            await _videoFacade.RegisterVideoLike(like);
            await _videoFacade.CountUserVideoInteraction(new UserTagInteraction() { BlobStorageId = like.BlobStorageId});
        }

        public async Task<User> CreateUser(User user)
            => await _userRepository.CreateUser(user);

        public async Task<User> GetUserOnRefreshToken(string refreshToken)
            => await _userRepository.GetUserOnRefreshToken(refreshToken);

        public async Task<UserDao> GetUserByVideoBlobId(string blobId)
            => await _userRepository.GetUserByVideoBlobId(blobId);

        public async Task<bool> UserExists(string email) 
            => await _userRepository.UserExists(email);

        public async Task CreateTokensOnUser(string email, string refreshToken)
            => await _userRepository.CreateTokensOnUser(email, refreshToken);

        public async Task<bool> IsRefreshTokenValid(string refreshToken)
            => await _userRepository.IsRefreshTokenValid(refreshToken);

        public async Task<List<UserTagInteractionDao>> GetUserTagInteractions()
            => await _userRepository.GetUserTagInteractions();

        public async Task RemoveRefreshToken(string refreshToken)
            => await _userRepository.RemoveRefreshToken(refreshToken);
    }
}
