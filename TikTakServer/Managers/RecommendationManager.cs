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
        public List<string> GetRandomTagsBasedOnUserPreference()
        {
            List<UserTagInteractionDao> preferences = _userRepository.GetUserTagInteractions();
            if(preferences.Any(preference => preference.Tag == null))
                throw new Exception("User had no tags accociated with them. No interactions found");
            //foreach (var preference in preferences)
            //{
            //    if (preference.Tag == null)
            //        throw new Exception("User had no tags accociated with them. No interactions found");
            //}


            int TotalWeightofPreferences = preferences.Sum(x => x.InteractionCount);
            double countryWeight = TotalWeightofPreferences * CountryWeight;


            List<string> videoTagResults = new List<string>();
            Random rnd = new Random();

            int countryRnd = rnd.Next(TotalWeightofPreferences);
            if (countryRnd <= countryWeight)
            {
                videoTagResults.Add("Denmark");
            }


            for (int I = videoTagResults.Count; I < videoCount; I++)
            {
                int rndmNumber = rnd.Next(TotalWeightofPreferences);
                int sumOfInteractions = 0;
                for (int j = 0; j < preferences.Count; j++)
                {

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
