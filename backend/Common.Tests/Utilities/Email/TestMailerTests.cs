using System.Net.Mail;
using TestUtilities;
using MailMessage = Common.Utilities.Email.MailMessage;

namespace Common.Tests.Utilities.Email;

public class TestMailerTests
{
    [Test]
    public void SendMailRecordsTheMessage()
    {
        var mailer = new TestMailer();
        var message = new MailMessage
        {
            To = "To",
            Subject = "Subject",
            Body = "Body"
        };

        mailer.Send(message);

        Assert.That(mailer.LastBody, Is.EqualTo("Body"));
        Assert.That(mailer.LastSubject, Is.EqualTo("Subject"));
        Assert.That(mailer.LastTo, Is.EqualTo("To"));
    }

    [Test]
    public void SendMailThrowsAnSmtpExceptionWhenTheToFieldIsSetToError()
    {
        var mailer = new TestMailer();
        var message = new MailMessage
        {
            To = "ERROR",
            Subject = "Subject",
            Body = "Body"
        };

        Assert.Throws<SmtpException>(() => mailer.Send(message));
    }

    [Test]
    public void SendMailRecordsNumberOfTimesCalled()
    {
        var mailer = new TestMailer();
        var message = new MailMessage
        {
            To = "To",
            Subject = "Subject",
            Body = "Body"
        };

        mailer.Send(message);
        mailer.Send(message);

        Assert.That(mailer.TimesCalled, Is.EqualTo(2));
    }

    [Test]
    public void SendMailThrowsArgumentExceptionWhenGivenNullMessageBody()
    {
        var mailer = new TestMailer();
        Assert.Throws<ArgumentException>(() => mailer.Send(new MailMessage { Body = null! }!));
    }
}