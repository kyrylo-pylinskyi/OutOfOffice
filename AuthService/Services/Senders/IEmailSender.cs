namespace AuthService.Services.Senders;

public interface IEmailSender
{
    Task SendEmailAsync(string recipient, string subject, string messageBody);
}