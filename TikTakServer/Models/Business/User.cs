namespace TikTakServer.Models.Business
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }

        public User(UserDao user)
        {
            UserName = user.UserName;
            Password = user.Password;
            DateOfBirth = user.DateOfBirth;
        }
    }
}
