using System.Net;
using System.Net.Mail;
using AuthService.Services.Options;
using Microsoft.Extensions.Options;

namespace AuthService.Services.Smtp;

public class SmtpClient : ISmtpClient
{
    private readonly SmtpSettings _smtpSettings;

    public SmtpClient(IOptions<SmtpSettings> emailSettings)
    {
        _smtpSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(MailMessage mailMessage)
    {
        var client = new System.Net.Mail.SmtpClient(_smtpSettings.SmtpServer)
        {
            Port = _smtpSettings.Port,
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl,
        };

        await client.SendMailAsync(mailMessage);
    }
}
