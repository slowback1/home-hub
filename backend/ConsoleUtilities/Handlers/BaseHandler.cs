using Common.Interfaces;

namespace ConsoleUtilities.Handlers;

public abstract class BaseHandler(ICrudFactory crudFactory) : IHandler
{
    protected ICrudFactory CrudFactory { get; } = crudFactory;

    public abstract Task HandleAsync(string[] args);
    public abstract string GetHelpMessage();
    public abstract string GetName();
}