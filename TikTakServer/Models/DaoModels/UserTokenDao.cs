namespace TikTakServer.Models.DaoModels
{
    public class UserTokenDao
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }


        public int UserId { get; set; }
        public virtual UserDao User { get; set; }
    }
}
