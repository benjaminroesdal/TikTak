using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using TikTakServer.Database;
using TikTakServer.Models;
using TikTakServer.Models.Business;

namespace TikTakServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TikTakContext _context;
        public UserRepository(TikTakContext context)
        {
            _context = context;
        }

        public async Task<UserDao> CreateUser(UserDao user)
        public List<UserTagInteractionDao> GetUserTagInteractions(int userId)
        {
            var userDao = await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserDao> GetUser(string email)
        {
            var userDao = _context.Users.First(e => e.Email == email);
            return userDao;
        }

        public async Task<UserDao> ValidateRefreshToken(string refreshToken)
        {
            var token = _context.Tokens.Where(e => e.RefreshToken == refreshToken).FirstOrDefault();
            if (token.RefreshTokenExpiry < DateTime.Now)
            {
                token.RefreshTokenExpiry = DateTime.MinValue;
                token.RefreshToken = null;
                await _context.SaveChangesAsync();
                throw new UnauthorizedAccessException("The provided refresh token has expired, please log in again.");
            }
            return await _context.Users.Where(e => e.Token.RefreshToken == refreshToken).FirstOrDefaultAsync();
        }

        public async Task InvalidateRefreshToken(string refreshToken)
        {
            var user = await _context.Users.Where(e => e.Token.RefreshToken == refreshToken).FirstOrDefaultAsync();
            _context.Tokens.Remove(user.Token);
            await _context.SaveChangesAsync();
            var userInteractions = _context.UserTagsInteractions.Where(i => i.UserId == userId).OrderBy(x => x.InteractionCount).ToList();
            return userInteractions;
        }
    }
}
