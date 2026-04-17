using System.Net;
using System.Net.Mail;

namespace Common.Utilities.Email;

public class Emailer : IMailer
{
    private readonly EmailSettings _settings;
    private readonly ISmtpClient _smtpClient;

    public Emailer(EmailSettings settings)
    {
        _settings = settings;
        _smtpClient = new SmtpClientWrapper(_settings.SmtpHost);
    }

    public Emailer(EmailSettings settings, ISmtpClient smtpClient)
    {
        _settings = settings;
        _smtpClient = smtpClient;
    }

    public void Send(MailMessage message)
    {
        _smtpClient.Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword);
        _smtpClient.EnableSsl = true;

        var from = new MailAddress(_settings.FromAddress, _settings.FromName);
        var to = new MailAddress(message.To);

        var mail = new System.Net.Mail.MailMessage(from, to)
        {
            Subject = message.Subject,
            Body = message.Body
        };

        _smtpClient.Send(mail);
    }
}