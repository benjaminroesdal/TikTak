using TikTakServer.Models;
using TikTakServer.Repositories;

namespace TikTakServer.Managers
{
    public class RecommendationManager : IRecommendationManager
    {
        private readonly IUserRepository _userRepository;
        private static int videoCount = 3;

        public RecommendationManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<TagDao> GetRandomTagsBasedOnUserPreference(int userId)
        {

            List<UserTagInteractionDao> preferences = _userRepository.GetUserTagInteractions(userId);
            //var userPreferences = new UserPreferences { TagScores = new Dictionary<TagDao, double>() };

            //foreach (var tag in preferences)
            //    userPreferences.TagScores.Add(tag.Tag, tag.InteractionCount);

            List<TagDao> videoTagResults = new List<TagDao>();
            Random rnd = new Random();
            int TotalWeightofPreferences = preferences.Sum(x => x.InteractionCount);

            
            for (int I = 0; I < videoCount; I++)
            {
                int rndmNumber = rnd.Next(TotalWeightofPreferences);
                //Used to traverse down the list of tags that the user has interacted the most with
                int sumOfInteractions = 0;
                foreach (var interaction in preferences)
                {
                    sumOfInteractions += interaction.InteractionCount;
                    if (rndmNumber < sumOfInteractions)
                    {
                        videoTagResults.Add(interaction.Tag);
                        break;
                    }
                }
            }
            return videoTagResults;
        }
    }

    //public class UserPreferences
    //{
    //    public Dictionary<TagDao, double> TagScores { get; set; }
    //}

    //public class Recommendation
    //{
    //    public TagDao RecommendedTag { get; set; }
    //    public double Score { get; set; }
    //}
}
