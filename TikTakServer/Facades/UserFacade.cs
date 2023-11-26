using TikTakServer.Models.Business;
using TikTakServer.Repositories;

namespace TikTakServer.Facades
{
    public class UserFacade:IUserFacade
    {
        private readonly IVideoRepository _videoRepository;
        public UserFacade(IVideoRepository videoRepository, IUserRepository userRepository)
        {
            _videoRepository = videoRepository;
        }

        public Task CountUserTagInteraction(UserTagInteraction interaction)
        {
            _videoRepository.CountUserVideoInteraction(interaction);
            return Task.CompletedTask;
        }

        public Task RegisterVideoLike(Like like)
        {
            _videoRepository.RegisterVideoLike(like);
            return Task.CompletedTask;
        }
    }
}
