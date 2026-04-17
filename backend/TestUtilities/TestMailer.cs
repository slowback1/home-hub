using System;
using System.Net.Mail;
using Common.Utilities.Email;
using MailMessage = Common.Utilities.Email.MailMessage;

namespace TestUtilities;

public class TestMailer : IMailer
{
    public string? LastTo { get; private set; }
    public string? LastSubject { get; private set; }
    public string? LastBody { get; private set; }
    public int TimesCalled { get; private set; }

    public void Send(MailMessage message)
    {
        LastTo = message.To;
        LastSubject = message.Subject;
        LastBody = message.Body;
        TimesCalled++;

        if (message.To == "ERROR") throw new SmtpException();
        if (string.IsNullOrEmpty(message.Body)) throw new ArgumentException();
    }
}