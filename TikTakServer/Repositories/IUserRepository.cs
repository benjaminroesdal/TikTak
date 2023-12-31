﻿using TikTakServer.Models.Business;
using TikTakServer.Models.DaoModels;

namespace TikTakServer.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<bool> IsRefreshTokenValid(string refreshToken);
        Task<User> GetUser(string email);
        Task CreateTokensOnUser(string email, string refreshToken);
        Task<bool> UserExists(string email);
        Task<List<UserTagInteractionModel>> GetUserTagInteractions();
        Task<User> GetUserByVideoBlobId(string blobId);
        Task RemoveRefreshToken(string refreshToken);
        Task<User> GetUserOnRefreshToken(string refreshToken);
    }
}
