using System.Net;
using System.Net.Mail;

namespace Common.Utilities.Email;

public class SmtpClientWrapper(string host) : ISmtpClient
{
    private readonly SmtpClient _client = new(host);

    public ICredentialsByHost Credentials
    {
        get => _client.Credentials;
        set => _client.Credentials = value;
    }

    public bool EnableSsl
    {
        get => _client.EnableSsl;
        set => _client.EnableSsl = value;
    }

    public void Send(System.Net.Mail.MailMessage message)
    {
        _client.Send(message);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}