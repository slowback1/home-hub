using Common.Utilities.Email;
using Common.Utilities.Messaging;
using TestUtilities;

namespace Common.Tests.Utilities.Email;

public class MailServiceTests
{
    private MailService MailService { get; set; }
    private TestMailer Mailer { get; set; }

    [SetUp]
    public void SetUp()
    {
        Mailer = new TestMailer();
        MailService = new MailService(Mailer);
    }

    [TearDown]
    public void TearDown()
    {
        MessageBus.ClearMessages();
        MessageBus.ClearSubscribers();
    }

    [Test]
    public async Task SendsMailWhenOnMessageIsCalled()
    {
        var message = new MailMessage
        {
            To = "To",
            Subject = "Subject",
            Body = "Body"
        };

        await MailService.OnMessage(message);

        Assert.That(Mailer.LastTo, Is.EqualTo("To"));
        Assert.That(Mailer.LastSubject, Is.EqualTo("Subject"));
        Assert.That(Mailer.LastBody, Is.EqualTo("Body"));
    }

    [Test]
    public async Task SendsMailWhenCalledThroughTheMessageBus()
    {
        var message = new MailMessage
        {
            To = "To",
            Subject = "Subject",
            Body = "Body"
        };

        await MessageBus.PublishAsync(Messages.SendEmail, message);

        Assert.That(Mailer.LastTo, Is.EqualTo("To"));
        Assert.That(Mailer.LastSubject, Is.EqualTo("Subject"));
        Assert.That(Mailer.LastBody, Is.EqualTo("Body"));
    }

    [Test]
    public async Task SendMailRetriesThreeTimesWhenSmtpExceptionIsThrown()
    {
        var message = new MailMessage
        {
            To = "ERROR",
            Subject = "Subject",
            Body = "Body"
        };

        await MailService.OnMessage(message);

        Assert.That(Mailer.TimesCalled, Is.EqualTo(3));
    }

    [Test]
    public async Task SendMailSendsAMessageToLogWhenSmtpIsThrownThreeTimes()
    {
        var message = new MailMessage
        {
            To = "ERROR",
            Subject = "Subject",
            Body = "Body"
        };

        await MailService.OnMessage(message);

        var lastMessage = MessageBus.GetLastMessage<string>(Messages.LogMessage);

        Assert.That(lastMessage, Is.EqualTo("Failed to send email to ERROR after 3 attempts."));
    }

    [Test]
    public async Task SendMailDoesNotRetryWhenAnyOtherExceptionTypeIsThrown()
    {
        var message = new MailMessage
        {
            To = "To",
            Subject = "Subject",
            Body = null!
        };

        await MailService.OnMessage(message);

        Assert.That(Mailer.TimesCalled, Is.EqualTo(1));
    }

    [Test]
    public async Task SendMailLogsErrorWhenAnyOtherExceptionTypeIsThrown()
    {
        var message = new MailMessage
        {
            To = "To",
            Subject = "Subject",
            Body = null!
        };

        await MailService.OnMessage(message);

        var lastMessage = MessageBus.GetLastMessage<string>(Messages.LogMessage);

        Assert.That(lastMessage, Contains.Substring("Value does not fall within the expected range."));
    }
}