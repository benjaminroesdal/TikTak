using TikTakServer.Models.Business;
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserRepository _userRepository;
        public UserFacade(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> CreateUser(User user)
            => await _userRepository.CreateUser(user);

        public async Task<User> GetUserOnRefreshToken(string refreshToken)
            => await _userRepository.GetUserOnRefreshToken(refreshToken);

        public async Task<User> GetUser(string email)
            => await _userRepository.GetUser(email);

        public async Task<User> GetUserByVideoBlobId(string blobId)
            => await _userRepository.GetUserByVideoBlobId(blobId);

        public async Task<bool> UserExists(string email) 
            => await _userRepository.UserExists(email);

        public async Task CreateTokensOnUser(string email, string refreshToken)
            => await _userRepository.CreateTokensOnUser(email, refreshToken);

        public async Task<bool> IsRefreshTokenValid(string refreshToken)
            => await _userRepository.IsRefreshTokenValid(refreshToken);

        public async Task<List<UserTagInteractionModel>> GetUserTagInteractions()
            => await _userRepository.GetUserTagInteractions();

        public async Task RemoveRefreshToken(string refreshToken)
            => await _userRepository.RemoveRefreshToken(refreshToken);
    }
}
