using SchurkoPortfolio.Core.Interfaces;
using SchurkoPortfolio.Core.Model;
using SchurkoPortfolio.Core.Utils;
using System.Net;
using System.Net.Mail;

namespace SchurkoPortfolio.Core.Services
{
    public class EmailService : ISmtpEmailService
    {

        public EmailService(SmtpSetting settings)
        {
            SmtpServer = settings.SmtpServer;
            Email = settings.Email;
            Password = settings.Password;
        }

        public string SmtpServer { get; }
        public string Email { get; }
        public string Password { get; } = string.Empty;



        public async Task<bool> SendEmailAsync(string email, string title, string body)
        {
            try
            {
                var msg = new MailMessage(email, Email, title, $"<p>{body}</p>")
                {
                    IsBodyHtml = true
                };

                using var client = new SmtpClient(SmtpServer, 587)
                {
                    Credentials = new NetworkCredential(Email, Password),
                    EnableSsl = true
                };

                await client.SendMailAsync(msg).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.LogError("Error sending email: {Message}", ex.Message);
                return false;
            }
        }
    }
}
