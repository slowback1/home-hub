using Common.Utilities.Messaging;

namespace Common.Tests.Utilities.Messaging;

public class TestMessage
{
	public string Message { get; init; } = string.Empty;
	public int Value { get; set; }
}

public class MessageBusTests
{
	[TearDown]
	public void ClearSubscribers()
	{
		MessageBus.ClearSubscribers();
	}

	[Test]
	public void CanSubscribeToAMessageAndReceiveIt()
	{
		var message = "test";
		var receivedMessage = "";

		MessageBus.Subscribe<string>(message, m => receivedMessage = m);
		MessageBus.Publish(message, message);

		Assert.That(receivedMessage, Is.SameAs(message));
	}

	[Test]
	public void CanHandleMultipleSubscribersToTheSameMessage()
	{
		var message = "test";
		var receivedMessage1 = "";
		var receivedMessage2 = "";

		MessageBus.Subscribe<string>(message, m => receivedMessage1 = m);
		MessageBus.Subscribe<string>(message, m => receivedMessage2 = m);
		MessageBus.Publish(message, message);

		Assert.That(receivedMessage1, Is.SameAs(message));
		Assert.That(receivedMessage2, Is.SameAs(message));
	}

	[Test]
	public void DoesntBreakWhenPublishingToAnUnsubscribedMessage()
	{
		var message = "i_have_no_subscribers";
		var receivedMessage = "";

		MessageBus.Publish(message, message);

		Assert.That(receivedMessage, Is.Empty);
	}

	[Test]
	public async Task CanHandleAnAsyncAction()
	{
		var message = "test";
		var receivedMessage = "";

		MessageBus.Subscribe<string>(message, async void (m) =>
		{
			try
			{
				await Task.Delay(100);
				receivedMessage = m;
			}
			catch
			{
				// ignored
			}
		});

		await MessageBus.PublishAsync(message, message);

		await Task.Delay(200);

		Assert.That(receivedMessage, Is.SameAs(message));
	}

	[Test]
	public async Task PublishHasAFunctioningAsyncOverride()
	{
		var message = "test";
		var receivedMessage = "";

		MessageBus.Subscribe<string>(message, m => { receivedMessage = m; });

		await MessageBus.PublishAsync(message, message);

		Assert.That(receivedMessage, Is.SameAs(message));
	}

	[Test]
	public void CanHandleComplexTypes()
	{
		var message = new TestMessage { Message = "complex", Value = 42 };
		TestMessage? receivedMessage = null;

		MessageBus.Subscribe<TestMessage>(message.Message, m => receivedMessage = m);
		MessageBus.Publish(message.Message, message);

		Assert.That(receivedMessage, Is.SameAs(message));
	}

	[Test]
	public void CanClearSubscribers()
	{
		var message = "test";
		var receivedMessage = "";

		MessageBus.Subscribe<string>(message, m => receivedMessage = m);
		MessageBus.ClearSubscribers();

		MessageBus.Publish(message, message);

		Assert.That(receivedMessage, Is.Empty);
	}

	[Test]
	public void CanUnsubscribe()
	{
		var message = "test";
		var receivedMessage = "";

		var unsubscribe = MessageBus.Subscribe<string>(message, m => receivedMessage = m);

		unsubscribe();

		MessageBus.Publish(message, "hello world");

		Assert.That(receivedMessage, Is.Empty);
	}

	[Test]
	[Repeat(1000)]
	public void UnsubscribingDoesNotAlsoUnsubscribeOtherSubscriptions()
	{
		var message = "test";
		var receivedMessage1 = "";
		var receivedMessage2 = "";

		var unsubscribe1 = MessageBus.Subscribe<string>(message, m => receivedMessage1 = m);
		MessageBus.Subscribe<string>(message, m => receivedMessage2 = m);

		unsubscribe1();

		MessageBus.Publish(message, "hello world");

		Assert.That(receivedMessage1, Is.Empty);
		Assert.That(receivedMessage2, Is.SameAs("hello world"));
	}

	[Test]
	public void DoesExplodeWhenSubscribingToTheSameMessageWithMultipleDifferentTypes()
	{
		var message = "test";

		MessageBus.Subscribe<string>(message, m => _ = m);
		MessageBus.Subscribe<int>(message, m => _ = m.ToString());

		Assert.Throws<InvalidCastException>(() => MessageBus.Publish(message, 42));
	}

	[Test]
	public void CanGetLastMessage()
	{
		var message = "test";

		MessageBus.Publish(message, "hello world");

		Assert.That(MessageBus.GetLastMessage<string>(message), Is.SameAs("hello world"));
	}

	[Test]
	public async Task CanGetLastMessageAsync()
	{
		var message = "test";

		await MessageBus.PublishAsync(message, "hello world2");

		Assert.That(MessageBus.GetLastMessage<string>(message), Is.SameAs("hello world2"));
	}

	[Test]
	public void CanClearMessageBusMessages()
	{
		var message = "test";

		MessageBus.Publish(message, "hello world");
		MessageBus.ClearMessages();

		Assert.That(MessageBus.GetLastMessage<string>(message), Is.Null);
	}

	[Test]
	public void CanSubscribeToAllMessagesAtTheSameTime()
	{
		var message1 = "test1";
		var message2 = "test2";
		var receivedMessage1 = "";
		var receivedMessage2 = "";

		MessageBus.SubscribeToAllMessages<string>(m => { receivedMessage1 = m; });
		MessageBus.SubscribeToAllMessages<string>(m => { receivedMessage2 = m; });

		MessageBus.Publish(message1, message1);
		MessageBus.Publish(message2, message2);

		Assert.That(receivedMessage1, Is.SameAs(message2));
		Assert.That(receivedMessage2, Is.SameAs(message2));
	}

	[Test]
	public void CanUnsubscribeFromAllMessages()
	{
		var message1 = "test1";
		var message2 = "test2";
		var receivedMessage1 = "";
		var receivedMessage2 = "";

		var unsubscribe1 = MessageBus.SubscribeToAllMessages<string>(m => { receivedMessage1 = m; });
		var unused = MessageBus.SubscribeToAllMessages<string>(m => { receivedMessage2 = m; });

		unsubscribe1();

		MessageBus.Publish(message1, message1);
		MessageBus.Publish(message2, message2);

		Assert.That(receivedMessage1, Is.Empty);
		Assert.That(receivedMessage2, Is.SameAs(message2));
	}

	[Test]
	public void DoesNotExplodeWhenSubscribingToAllMessagesAndReceivesAComplexTypeThatItIsNotExpecting()
	{
		var message = new TestMessage { Message = "complex", Value = 42 };
		string? receivedMessage = null;

		MessageBus.SubscribeToAllMessages<string>(_ => { receivedMessage = "got it"; });

		MessageBus.Publish(message.Message, message);

		Assert.That(receivedMessage, Is.EqualTo("got it"));
	}

	[Test]
	public async Task CanHandleAnAsyncActionForAllMessages()
	{
		var message = "test";
		var receivedMessage = "";

		MessageBus.SubscribeToAllMessages<string>(async void (m) =>
		{
			try
			{
				await Task.Delay(100);
				receivedMessage = m;
			}
			catch
			{
				// ignored
			}
		});

		await MessageBus.PublishAsync(message, message);

		await Task.Delay(200);

		Assert.That(receivedMessage, Is.SameAs(message));
	}
}