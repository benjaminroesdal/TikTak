using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;
using TikTakServer.Facades;
using Azure;

namespace TikTakServer.Managers
{
    public class RecommendationManager : IRecommendationManager
    {
        private readonly IUserFacade _userFacade;

        private static readonly Random _random = new Random();
        private static int videoCount = 3;

        public RecommendationManager(IUserFacade userFacade)
        {
            _userFacade = userFacade;
        }
        public List<string> GetRandomTagsBasedOnUserPreference()
        {
            var preferences = _userFacade.GetUserTagInteractions();

            int TotalWeightofPreferences = preferences.Sum(x => x.InteractionCount);

            var videoTagResults = PopulateTagsList(preferences, TotalWeightofPreferences);

            return videoTagResults;
        }

        private List<string> PopulateTagsList(List<UserTagInteractionDao> interactions, int weight)
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
