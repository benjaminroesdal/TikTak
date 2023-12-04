using Microsoft.EntityFrameworkCore;
using TikTakServer.Database;
using TikTakServer.Models.DaoModels;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserRequestAndClaims _requestAndClaims;
        private readonly TikTakContext _context;
        public UserRepository(TikTakContext context, UserRequestAndClaims requestAndClaims)
        {
            _context = context;
            _requestAndClaims = requestAndClaims;
        }

        public async Task<User> CreateUser(User user)
        {
            var userDao = new UserDao(user);
            await _context.AddAsync(userDao);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Finds user based on provided email
        /// </summary>
        /// <param name="email">email to find user on</param>
        /// <returns>user model</returns>
        public async Task<User> GetUser(string email)
        {
            var userDao = await _context.Users.FirstAsync(e => e.Email == email);
            return new User(userDao, null);
        }

        /// <summary>
        /// Finds user based on provided refreshToken
        /// </summary>
        /// <param name="refreshToken">refreshToken to find user on</param>
        /// <returns>User model</returns>
        public async Task<User> GetUserOnRefreshToken(string refreshToken)
        {
            var userDao = await _context.Users.Where(e => e.Tokens.Any(x => x.RefreshToken == refreshToken)).FirstAsync();
            return new User(userDao, null);
        }

        /// <summary>
        /// Finds user based on provided blobId
        /// </summary>
        /// <param name="blobId">blobId to find user on.</param>
        /// <returns>User model</returns>
        public async Task<User> GetUserByVideoBlobId(string blobId)
        {
            var userDao = await _context.Users
                .Include(x => x.Videos)
                .Where(i => i.Videos.Where(e => e.BlobStorageId == blobId).Any())
                .FirstAsync();

            return new User(userDao, null);
        }

        /// <summary>
        /// Checks if user with provided email exists
        /// </summary>
        /// <param name="email">Email to find user on</param>
        /// <returns>True/False based on result of check</returns>
        public async Task<bool> UserExists(string email)
        {
            var userExist = await _context.Users.AnyAsync(e => e.Email == email);
            return userExist;
        }

        /// <summary>
        /// Remove refreshToken from user
        /// </summary>
        /// <param name="refreshToken">Refresh token to remove</param>
        public async Task RemoveRefreshToken(string refreshToken)
        {
            var token = await _context.Tokens.FirstAsync(e => e.RefreshToken == refreshToken);
            _context.Tokens.Remove(token);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds provided refreshToken on the user with the provided email.
        /// </summary>
        /// <param name="email">email on user to add token to.</param>
        /// <param name="refreshToken">token to add to user</param>
        public async Task CreateTokensOnUser(string email, string refreshToken)
        {
            var user = await _context.Users
                .Include(x => x.Tokens)
                .FirstAsync(e => e.Email == email);
            user.Tokens.Add(new UserTokenDao() { RefreshToken = refreshToken, RefreshTokenExpiry = DateTime.Now.AddDays(200), UserId = user.Id});
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if refreshToken provided exists and is valid.
        /// </summary>
        /// <param name="refreshToken">refreshToken to validate</param>
        /// <returns>true or false based on vailidity</returns>
        public async Task<bool> IsRefreshTokenValid(string refreshToken)
        {
            var token = await _context.Tokens
                .Where(e => e.RefreshToken == refreshToken)
                .FirstOrDefaultAsync();
            if (token == null)
            {
                throw new UnauthorizedAccessException("The provided refresh token is not valid");
            }
            if (token.RefreshTokenExpiry < DateTime.Now)
            {
                token.RefreshTokenExpiry = DateTime.MinValue;
                token.RefreshToken = null;
                await _context.SaveChangesAsync();
                throw new UnauthorizedAccessException("The provided refresh token has expired, please log in again.");
            }
            return true;
        }

        /// <summary>
        /// Gets a list of UserTagInteractions on the user in requestAndClaims
        /// </summary>
        /// <returns>List of userTagInteractions</returns>
        public async Task<List<UserTagInteractionModel>> GetUserTagInteractions()
        {
            var userInteractions = await _context.UserTagsInteractions
                .Include(tag => tag.Tag)
                .Where(i => i.User.Email == _requestAndClaims.Email)
                .OrderByDescending(x => x.InteractionCount)
                .ToListAsync();
            List<UserTagInteractionModel> userInteractionModels = new List<UserTagInteractionModel>();
            userInteractions.ForEach(x => userInteractionModels.Add(new UserTagInteractionModel() { InteractionCount = x.InteractionCount, Tag = new TagModel() { Name = x.Tag.Name } }));
            return userInteractionModels;
        }
    }
}
