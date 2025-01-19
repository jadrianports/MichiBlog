using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using MichiBlog.WebApp.Settings;
using MichiBlog.WebApp.Interfaces;

namespace MichiBlog.WebApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IConfiguration configuration,
                            IOptions<SmtpSettings> smtpSettings)
        {
            _configuration = configuration;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Contact Message from MichiBlog", _smtpSettings.Username));
            email.To.Add(new MailboxAddress("Email", to));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = message };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
