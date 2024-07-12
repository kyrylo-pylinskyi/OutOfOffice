using System.Net.Mail;
using AuthService.Services.Options;
using AuthService.Services.Senders;
using AuthService.Services.Smtp;
using Microsoft.Extensions.Options;
using Moq;
using EmailSender = AuthService.Services.Senders.EmailSender;

namespace AuthServiceTests.Services;

public class EmailSenderTests
{
    [Fact]
    public async Task EmailSender_SendEmailAsync_ShouldSendEmail()
    {
        // Arrange
        var emailSettings = new SmtpSettings
        {
            SmtpServer = "smtp.example.com",
            Port = 587,
            Username = "test_username",
            Password = "test_password",
            EnableSsl = true,
            SenderEmail = "sender@example.com",
            SenderName = "Sender Name"
        };

        var mockOptions = new Mock<IOptions<SmtpSettings>>();
        mockOptions.Setup(o => o.Value).Returns(emailSettings);

        var smtpClientMock = new Mock<ISmtpClient>();
        smtpClientMock.Setup(c => c.SendEmailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);
        var emailSender = new EmailSender(smtpClientMock.Object, mockOptions.Object);

        // Act
        await emailSender.SendEmailAsync("recipient@example.com", "Test Subject", "Test Message");

        // Assert
        smtpClientMock.Verify(c => c.SendEmailAsync(It.Is<MailMessage>(msg =>
            msg.From != null &&
            msg.From.Address == emailSettings.SenderEmail &&
            msg.Subject == "Test Subject" &&
            msg.Body == "Test Message" &&
            msg.To.Contains(new MailAddress("recipient@example.com"))
        )), Times.Once);
    }
}