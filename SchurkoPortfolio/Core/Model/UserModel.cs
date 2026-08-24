namespace SchurkoPortfolio.Core.Model
{
    public class UserModel
    {
        public string NameIdentifier { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}
