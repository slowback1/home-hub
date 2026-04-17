using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Utilities.Messaging;

public static class MessageBus
{
	private static readonly Dictionary<string, object> LastMessages = new();

	private static readonly Dictionary<string, List<MessageBusAction>> Subscribers = new();

	private static List<MessageBusAction> _allMessageSubscribers = new();

	public static Action Subscribe<T>(string message, Action<T> action)
	{
		var function = ConvertToFunction(action);

		if (!Subscribers.TryGetValue(message, out var value))
			Subscribers.Add(message, [function]);
		else
			value.Add(function);

		return () => { Subscribers[message] = Subscribers[message].Where(f => f.Id != function.Id).ToList(); };
	}

	public static Action SubscribeToAllMessages<T>(Action<T> action)
	{
		var function = ConvertToFunction(action);
		_allMessageSubscribers.Add(function);

		return () => { _allMessageSubscribers = _allMessageSubscribers.Where(f => f.Id != function.Id).ToList(); };
	}

	private static MessageBusAction ConvertToFunction<T>(Action<T> action)
	{
		Task MessageAction(object? o)
		{
			action((o != null ? (T)o : default)!);
			return Task.CompletedTask;
		}

		return new MessageBusAction
		{
			Id = Guid.NewGuid().ToString(),
			Action = MessageAction
		};
	}

	public static void Publish<T>(string message, T payload)
	{
		AddToDictionary(message, payload);

		foreach (var action in _allMessageSubscribers) TryPublishMessage(action.Action, payload);

		if (!Subscribers.TryGetValue(message, out var actions)) return;

		foreach (var action in actions) action.Action(payload!);
	}

	private static void TryPublishMessage<T>(Func<object, Task> action, T payload)
	{
		try
		{
			action(payload!);
		}
		catch
		{
			action(default(T)!);
		}
	}

	private static void AddToDictionary<T>(string message, T payload)
	{
		LastMessages[message] = payload!;
	}

	public static async Task PublishAsync<T>(string message, T payload)
	{
		AddToDictionary(message, payload);
		foreach (var action in _allMessageSubscribers) TryPublishMessage(action.Action, payload);

		if (!Subscribers.TryGetValue(message, out var actions)) return;

		foreach (var action in actions) await Task.Run(() => action.Action(payload!));
	}

	public static T? GetLastMessage<T>(string message)
	{
		return LastMessages.TryGetValue(message, out var value) ? (T)value : default;
	}

	public static void ClearSubscribers()
	{
		Subscribers.Clear();
		_allMessageSubscribers.Clear();
	}

	public static void ClearMessages()
	{
		LastMessages.Clear();
	}
}