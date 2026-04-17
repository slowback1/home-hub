namespace Common.Utilities.Email;

public interface IMailer
{
    void Send(MailMessage message);
}