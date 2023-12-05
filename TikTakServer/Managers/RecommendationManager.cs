using TikTakServer.Facades;
using TikTakServer.Models.Business;

namespace TikTakServer.Managers
{
    public class RecommendationManager : IRecommendationManager
    {
        private readonly IUserFacade _userFacade;

        private static readonly Random _random = new Random();
        private static int videoCount = 5;

        public RecommendationManager(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        /// <summary>
        /// Find a list of random tags based on user interaction
        /// </summary>
        /// <returns>List of random tags based on user interactions</returns>
        public async Task<List<string>> GetRandomTagsBasedOnUserPreference()
        {
            var preferences = await _userFacade.GetUserTagInteractions();

            int TotalWeightofPreferences = preferences.Sum(x => x.InteractionCount);

            var videoTagResults = PopulateTagsList(preferences, TotalWeightofPreferences);

            return videoTagResults;
        }

        /// <summary>
        /// Populates a list with tags based on weight and interactions.
        /// </summary>
        /// <param name="interactions">UserTagInteractions from the user</param>
        /// <param name="weight">weight to base tag population on</param>
        /// <returns></returns>
        private List<string> PopulateTagsList(List<UserTagInteractionModel> interactions, int weight)
        {
            var listOfTags = new List<string>();
            for (int I = listOfTags.Count; I < videoCount; I++)
            {
                int rndmNumber = _random.Next(weight);
                int sumOfInteractions = 0;
                for (int j = 0; j < interactions.Count; j++)
                {
                    if (interactions[j].InteractionCount <= 0)
                    {
                        listOfTags.Add(interactions[_random.Next(0, interactions.Count)].Tag.Name);
                        break;
                    }


                    sumOfInteractions += interactions[j].InteractionCount;
                    if (rndmNumber < sumOfInteractions)
                    {
                        listOfTags.Add(interactions[j].Tag.Name);
                        break;
                    }
                }
            }
            return listOfTags;
        }
    }
}
