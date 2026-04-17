namespace ConsoleUtilities.Handlers;

public interface IHandler
{
    Task HandleAsync(string[] args);
}