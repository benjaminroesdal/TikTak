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

        public async Task<UserDao> CreateUser(UserDao user)
        {
            var userDao = await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserDao> GetUser(string email)
        {
            return _context.Users.First(e => e.Email == email);
        }

        public async Task<UserDao> GetUserByVideoBlobId(string blobId)
        {
            return _context.Users.Include(x => x.Videos).Where(i => i.Videos.Where(e => e.BlobStorageId == blobId).Any()).First();
        }

        public async Task<bool> UserExists(string email)
            {
            var see = await _context.Users.AnyAsync(e => e.Email == email);
            return see;
        }

        public async Task CreateTokensOnUser(string email, string refreshToken)
        {
            var user = _context.Users.Include(x => x.Tokens).First(e => e.Email == email);
            user.Tokens.Add(new UserTokenDao() { RefreshToken = refreshToken, RefreshTokenExpiry = DateTime.Now.AddDays(200), UserId = user.Id});
            await _context.SaveChangesAsync();
        }

        public async Task<UserDao> ValidateRefreshToken(string refreshToken)
        {
            var token = _context.Tokens.Where(e => e.RefreshToken == refreshToken).FirstOrDefault();
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
            return await _context.Users.Where(e => e.Tokens.Any(e => e.RefreshToken == refreshToken)).FirstOrDefaultAsync();
        }

        public List<UserTagInteractionDao> GetUserTagInteractions()
        {
            var userInteractions = _context.UserTagsInteractions.Include(tag => tag.Tag).Where(i => i.UserId == int.Parse(_requestAndClaims.UserId)).OrderByDescending(x => x.InteractionCount).ToList();
            return userInteractions;
        }
    }
}
