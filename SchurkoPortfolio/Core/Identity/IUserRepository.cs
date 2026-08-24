using SchurkoPortfolio.Core.Model;

namespace SchurkoPortfolio.Core.Identity
{
    public interface IUserRepository
    {
        public bool IsAuthenticated(string username, string password);

        public UserModel GetByUsernameAndPassword(string username, string password);
    }
}
