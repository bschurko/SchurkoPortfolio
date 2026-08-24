using SchurkoPortfolio.Core.Model;

namespace SchurkoPortfolio.Core.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly List<UserModel> _users = new List<UserModel>
        {
            // Password: Password123! (Non-Hashed)
            new UserModel { NameIdentifier = "1945", Email = "brettschurko@gmail.com", Password =  "a109e36947ad56de1dca1cc49f0ef8ac9ad9a7b1aa0df41fb3c4cb73c1ff01ea", Role = "Administrator" }
        };

        public UserRepository()
        {
        }

        public bool IsAuthenticated(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password.ToSha256());

            return user != null;
        }

        public UserModel GetByUsernameAndPassword(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password.ToSha256());

            return user;
        }
    }
}
