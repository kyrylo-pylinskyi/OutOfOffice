using System.Net.Mail;

namespace AuthService.Services.Smtp;

public interface ISmtpClient
{
    Task SendEmailAsync(MailMessage mailMessage);
    MailAddress GetMailAddress();
}