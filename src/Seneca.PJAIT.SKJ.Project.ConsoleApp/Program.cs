using System.CommandLine;
using Seneca.PJAIT.SKJ.Project.ConsoleApp;

Option<int> tcpPortOption = new(
    name: "--tcpport",
    description: "The TCP port number")
{
    IsRequired = true
};

Option<string> recordOption = new(
    name: "--record",
    description: "The record to store")
{
    IsRequired = true
};

Option<IEnumerable<string>> connectedNodesOption = new(
    name: "--connect",
    description: "List of connected nodes")
{
    AllowMultipleArgumentsPerToken = true,
    IsRequired = false
};

RootCommand rootCommand = new(description: "Seneca.PJAIT.SKJ.Project.ConsoleApp")
{
    tcpPortOption,
    recordOption,
    connectedNodesOption,
};

rootCommand.SetHandler(async (tcpPortOptionValue, record, nodes) =>
    {
        await new DatabaseNode().Run(tcpPortOptionValue, record, nodes);
    },
    tcpPortOption,
    new KeyValueRecordBinder(recordOption),
    new NodeBinder(connectedNodesOption));

await rootCommand.InvokeAsync(args);
