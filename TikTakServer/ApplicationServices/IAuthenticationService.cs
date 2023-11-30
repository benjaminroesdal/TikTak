﻿using TikTakServer.Models.Business;

namespace TikTakServer.ApplicationServices
{
    public interface IAuthenticationService
    {
        Task<User> RefreshAccessToken(string refreshToken);
        Task<User> Login(string googleAccessToken, string name, string imgUrl, double longi, double lati);
        Task Logout(string refreshToken);
    }
}
