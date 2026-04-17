using System;
using System.Threading.Tasks;

namespace Common.Utilities.Messaging;

internal class MessageBusAction
{
    public string Id { get; set; } = string.Empty;
    public Func<object, Task> Action { get; set; } = _ => Task.CompletedTask;
}