namespace SchurkoPortfolio.Core.Interfaces
{
    public interface ISmtpEmailService
    {
        public Task<bool> SendEmailAsync(string email, string title, string body);
    }
}
