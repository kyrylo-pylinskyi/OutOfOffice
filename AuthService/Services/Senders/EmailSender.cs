using System.Net.Mail;
using AuthService.Services.Options;
using AuthService.Services.Smtp;
using Microsoft.Extensions.Options;

namespace AuthService.Services.Senders;

public class EmailSender : IEmailSender
{
    private readonly ISmtpClient _smtpClient;
    public EmailSender(ISmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public async Task SendEmailAsync(string recipient, string subject, string messageBody, bool isBodyHtml = false)
    {
        var message = new MailMessage
        {
            From = _smtpClient.GetMailAddress(),
            Subject = subject,
            Body = messageBody,
            IsBodyHtml = false,
        };
        message.To.Add(new MailAddress(recipient));

        await _smtpClient.SendEmailAsync(message);
    }
}