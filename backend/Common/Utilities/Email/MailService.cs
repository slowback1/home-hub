using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Common.Utilities.Messaging;

namespace Common.Utilities.Email;

public class MailService(IMailer mailer) : MessageBusListener<MailMessage>(Messages.SendEmail)
{
    public override Task OnMessage(MailMessage message)
    {
        TrySendMessage(message);

        return Task.CompletedTask;
    }

    private void TrySendMessage(MailMessage message, int timesCalled = 1)
    {
        try
        {
            mailer.Send(message);
        }
        catch (SmtpException)
        {
            if (timesCalled < 3) TrySendMessage(message, timesCalled + 1);
            else MessageBus.Publish(Messages.LogMessage, $"Failed to send email to {message.To} after 3 attempts.");
        }
        catch (Exception e)
        {
            MessageBus.Publish(Messages.LogMessage, e.ToString());
        }
    }
}