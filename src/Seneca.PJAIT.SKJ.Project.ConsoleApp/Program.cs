using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Seneca.PJAIT.SKJ.Project.ConsoleApp;

RootCommand rootCommand = RootCommandFactory.CreateRootCommand(
    new DatabaseNode(),
    (node, port, record, nodes) => node.Run(port, record, nodes));

await rootCommand.InvokeAsync(args);
