using System.Net.Mail;
using AuthService.Services.Options;
using AuthService.Services.Smtp;
using Microsoft.Extensions.Options;

namespace AuthService.Services.Senders;

public class EmailSender : IEmailSender
{
    private readonly ISmtpClient _smtpClient;
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings, ISmtpClient smtpClient)
    {
        _smtpSettings = smtpSettings.Value;
        _smtpClient = smtpClient;
    }

    public async Task SendEmailAsync(string recipient, string subject, string messageBody)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
            Subject = subject,
            Body = messageBody,
            IsBodyHtml = false,
        };
        message.To.Add(new MailAddress(recipient));

        await _smtpClient.SendEmailAsync(message);
    }
}
