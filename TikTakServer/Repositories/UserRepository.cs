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

        public async Task<User> GetUserOnRefreshToken(string refreshToken)
        {
            var userDao = await _context.Users.FirstAsync(e => e.Tokens.Where(x => x.RefreshToken == refreshToken).Any());
            return new User(userDao, refreshToken);
        }

        public async Task<UserDao> GetUserByVideoBlobId(string blobId)
        {
            return await _context.Users
                .Include(x => x.Videos)
                .Where(i => i.Videos.Where(e => e.BlobStorageId == blobId).Any())
                .FirstAsync();
        }

        public async Task<bool> UserExists(string email)
            {
            var see = await _context.Users.AnyAsync(e => e.Email == email);
            return see;
        }

        public async Task RemoveRefreshToken(string refreshToken)
        {
            var token = await _context.Tokens.FirstAsync(e => e.RefreshToken == refreshToken);
            _context.Tokens.Remove(token);
            await _context.SaveChangesAsync();
        }

        public async Task CreateTokensOnUser(string email, string refreshToken)
        {
            var user = await _context.Users
                .Include(x => x.Tokens)
                .FirstAsync(e => e.Email == email);
            user.Tokens.Add(new UserTokenDao() { RefreshToken = refreshToken, RefreshTokenExpiry = DateTime.Now.AddDays(200), UserId = user.Id});
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsRefreshTokenValid(string refreshToken)
        {
            var token = await _context.Tokens
                .Where(e => e.RefreshToken == refreshToken && e.User.Email == this._requestAndClaims.Email)
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

        public async Task<List<UserTagInteractionDao>> GetUserTagInteractions()
        {
            var userInteractions = await _context.UserTagsInteractions
                .Include(tag => tag.Tag)
                .Where(i => i.User.Email == _requestAndClaims.Email)
                .OrderByDescending(x => x.InteractionCount)
                .ToListAsync();
            return userInteractions;
        }
    }
}
