using TikTakServer.Models;
using TikTakServer.Repositories;

namespace TikTakServer.Managers
{
    public class RecommendationManager : IRecommendationManager
    {
        private readonly IUserRepository _userRepository;
        private static int videoCount = 3;
        private static double CountryWeight = 0.8;

        public RecommendationManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<string> GetRandomTagsBasedOnUserPreference(int userId)
        {
            List<UserTagInteractionDao> preferences = _userRepository.GetUserTagInteractions(userId);
            int TotalWeightofPreferences = preferences.Sum(x => x.InteractionCount);
            double countryWeight = TotalWeightofPreferences * CountryWeight;


            List<string> videoTagResults = new List<string>();
            Random rnd = new Random();


            
            for (int I = 0; I < videoCount; I++)
            {

                int countryRnd = rnd.Next(TotalWeightofPreferences);
                if (countryRnd <= countryWeight)
                {
                    videoTagResults.Add("Denmark");
                    I++;
                    break;
                }

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
