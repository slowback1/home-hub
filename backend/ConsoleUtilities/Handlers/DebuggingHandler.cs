using Common.Interfaces;

namespace ConsoleUtilities.Handlers;

public class DebuggingHandler(ICrudFactory crudFactory) : BaseHandler(crudFactory)
{
	public override Task HandleAsync(string[] args)
	{
		Console.WriteLine("DebuggingHandler invoked");

		Console.WriteLine("Arguments:");
		foreach (var arg in args) Console.WriteLine(arg);

		return Task.CompletedTask;
	}

	public override string GetHelpMessage()
	{
		return "Handler for debugging purposes. It prints the provided arguments to the console.";
	}

	public override string GetName()
	{
		return "Debugging";
	}
}