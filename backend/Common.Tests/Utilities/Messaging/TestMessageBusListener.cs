using Common.Utilities.Messaging;

namespace Common.Tests.Utilities.Messaging;

public class TestMessageBusListener() : MessageBusListener<string>("test_listener")
{
	public string LastMessage { get; private set; } = string.Empty;

	public override Task OnMessage(string message)
	{
		LastMessage = message;
		return Task.CompletedTask;
	}
}