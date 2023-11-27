using TikTakServer.Models;
using TikTakServer.Repositories;

namespace TikTakServer.Managers
{
    public class RecommendationManager : IRecommendationManager
    {
        private readonly IUserRepository _userRepository;
        private readonly UserRequestAndClaims _userRequestAndClaims;

        private static int videoCount = 3;
        private static double CountryWeight = 0.8;

        public RecommendationManager(IUserRepository userRepository, UserRequestAndClaims userRequestAndClaims)
        {
            _userRepository = userRepository;
            _userRequestAndClaims = userRequestAndClaims;
        }
        public List<string> GetRandomTagsBasedOnUserPreference()
        {
            List<UserTagInteractionDao> preferences = _userRepository.GetUserTagInteractions();

            int TotalWeightofPreferences = preferences.Sum(x => x.InteractionCount);
            double countryWeight = TotalWeightofPreferences * CountryWeight;


            List<string> videoTagResults = new List<string>();
            if(preferences.Count == 0)
            {
                for(int i = 0; i < videoCount; i++)
                {
                    videoTagResults.Add(_userRequestAndClaims.Country);
                }
                return videoTagResults;
            }

            Random rnd = new Random();

            int countryRnd = rnd.Next(TotalWeightofPreferences);
            if (countryRnd <= countryWeight)
            {
                videoTagResults.Add(_userRequestAndClaims.Country);
            }

            for (int I = videoTagResults.Count; I < videoCount; I++)
            {
                int rndmNumber = rnd.Next(TotalWeightofPreferences);
                int sumOfInteractions = 0;
                for (int j = 0; j < preferences.Count; j++)
                {
                    if (preferences[j].InteractionCount <= 0)
                    {
                        videoTagResults.Add(preferences[rnd.Next(0, preferences.Count)].Tag.Name);
                        break;
                    }


                    sumOfInteractions += preferences[j].InteractionCount;
                    if (rndmNumber < sumOfInteractions)
                    {
                        videoTagResults.Add(preferences[j].Tag.Name);
                        break;
                    }
                }
            }
            return videoTagResults;
        }
    }
}
