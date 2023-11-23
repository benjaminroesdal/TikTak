using TikTakServer.Models.Business;

namespace TikTakServer.Facades
{
    public interface IUserFacade
    {
        Task CountUserTagInteraction(UserTagInteraction interaction);
        Task RegisterVideoLike(Like like);
    }
}
