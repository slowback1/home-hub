using ConsoleUtilities.Handlers;

namespace ConsoleUtilities;

public class ConsoleMenu(HandlerFactory handlerFactory)
{
	private const int MaxItemsPerPage = 5;

	public void ShowMenu()
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine("Select an option:");
			var options = handlerFactory.GetHandlerOptions();
			DisplayOptionsWithPaging(options);
			Console.WriteLine("0. Exit");

			if (!TryGetUserChoice(out var choice))
			{
				ShowMessageAndWait("Invalid input. Press any key to try again...");
				continue;
			}

			if (choice == 0) break;
			if (!HandleMenuChoice(choice, options)) ShowMessageAndWait("Invalid choice. Press any key to try again...");
		}
	}

	private void DisplayOptionsWithPaging(IReadOnlyList<string> options)
	{
		for (var i = 0; i < options.Count; i++)
		{
			Console.WriteLine($"{i + 1}. {options[i]}");
			if ((i + 1) % MaxItemsPerPage == 0 && i != options.Count - 1)
			{
				ShowMessageAndWait("Press any key to see more...");
				Console.Clear();
			}
		}
	}

	private bool TryGetUserChoice(out int choice)
	{
		var input = Console.ReadLine();
		return int.TryParse(input, out choice);
	}

	private bool HandleMenuChoice(int choice, IReadOnlyList<string> options)
	{
		if (choice <= 0 || choice > options.Count)
			return false;
		var handlerName = options[choice - 1].Split(" - ")[0];
		var handler = handlerFactory.GetHandler(handlerName);
		Console.WriteLine($"You selected: {handlerName}");
		Console.WriteLine("Enter arguments separated by spaces:");
		var argsInput = Console.ReadLine();
		var args = argsInput?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? [];
		handler.HandleAsync(args).Wait();
		ShowMessageAndWait("Press any key to return to the menu...");
		return true;
	}

	private void ShowMessageAndWait(string message)
	{
		Console.WriteLine(message);
		Console.ReadKey();
	}
}