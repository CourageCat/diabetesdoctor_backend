using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AuthService.Api.Infrastructures.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailConfig;

    public EmailService(IOptions<EmailSettings> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }

    public async Task<bool> SendMailAsync
        (string toEmail, string subject, string templateName, Dictionary<string, string> Body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_emailConfig.EmailUsername));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder();

        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", templateName);
        var template = File.ReadAllText(templatePath);

        bodyBuilder.HtmlBody = File.ReadAllText(templatePath);

        foreach (var placeholder in Body)
        {
            template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }

        bodyBuilder.HtmlBody = template;
        email.Body = bodyBuilder.ToMessageBody();

        try
        {
            var client = new SmtpClient();
            await client.ConnectAsync(_emailConfig.EmailHost, 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailConfig.EmailUsername, _emailConfig.EmailPassword);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            return false;
        }
    }
}