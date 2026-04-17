// See https://aka.ms/new-console-template for more information

using ConsoleUtilities;
using ConsoleUtilities.Handlers;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json",
        true)
    .Build();

var crudFactory = CrudFactoryMaker.ConfigureCrudFactory(config);

var handlerFactory = new HandlerFactory(crudFactory);

var consoleMenu = new ConsoleMenu(handlerFactory);
consoleMenu.ShowMenu();