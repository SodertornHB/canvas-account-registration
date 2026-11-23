using CanvasAccountRegistration.Logic.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Services
{
    public interface IMailService
    {
        Task Send(string from, string to, string subject, string htmlBody);
    }

    /// <summary>
    /// Sends e-mail through the SMTP server configured in the "Smtp" section of appsettings.
    /// Replace the registration of IMailService to use another mail provider.
    /// </summary>
    public class SmtpMailService : IMailService
    {
        private readonly SmtpSettings settings;

        public SmtpMailService(IOptions<SmtpSettings> options)
        {
            settings = options.Value;
        }

        public async Task Send(string from, string to, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(settings.Host))
                throw new InvalidOperationException("Smtp:Host is not configured.");

            using var message = new MailMessage(from, to, subject, htmlBody) { IsBodyHtml = true };
            using var client = new SmtpClient(settings.Host, settings.Port) { EnableSsl = settings.EnableSsl };
            if (!string.IsNullOrEmpty(settings.UserName))
                client.Credentials = new NetworkCredential(settings.UserName, settings.Password);
            await client.SendMailAsync(message);
        }
    }
}
