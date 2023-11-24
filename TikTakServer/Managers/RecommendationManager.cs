using TikTakServer.Models;
using TikTakServer.Repositories;

namespace TikTakServer.Managers
{
    public class RecommendationManager : IRecommendationManager
    {
        private readonly IUserRepository _userRepository;
        private static int videoCount = 3;
        private static double countryWeight = 0.8;

        public RecommendationManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<string> GetRandomTagsBasedOnUserPreference(int userId)
        {

            List<UserTagInteractionDao> preferences = _userRepository.GetUserTagInteractions(userId);

            List<string> videoTagResults = new List<string>();
            Random rnd = new Random();
            int TotalWeightofPreferences = preferences.Sum(x => x.InteractionCount);
            double country = TotalWeightofPreferences / countryWeight;
            
            for (int I = 0; I < videoCount; I++)
            {
                int rndmNumber = rnd.Next(TotalWeightofPreferences);
                int sumOfInteractions = 0;
                foreach (var interaction in preferences)
                {
                    sumOfInteractions += interaction.InteractionCount;
                    if (rndmNumber < sumOfInteractions)
                    {
                        videoTagResults.Add(interaction.Tag.Name);
                        break;
                    }
                }
            }
            return videoTagResults;
        }

        
    }
}
