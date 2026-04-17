using Common.Utilities.Messaging;

namespace Common.Tests.Utilities.Messaging;

public class TestStaticMessageBusListener : MessageBusListener<int>
{
    public static readonly TestStaticMessageBusListener Instance;

    static TestStaticMessageBusListener()
    {
        Instance ??= new TestStaticMessageBusListener();
    }

    private TestStaticMessageBusListener() : base("static_listener_test")
    {
    }

    public int LastMessage { get; private set; }

    public override Task OnMessage(int message)
    {
        LastMessage = message;
        return Task.CompletedTask;
    }
}