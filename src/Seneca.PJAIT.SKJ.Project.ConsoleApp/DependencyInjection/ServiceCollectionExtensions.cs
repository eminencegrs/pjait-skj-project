using Microsoft.Extensions.DependencyInjection;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Client;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Handlers.Internal;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Services;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.Storage;

namespace Seneca.PJAIT.SKJ.Project.ConsoleApp.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDependencies(this IServiceCollection services)
    {
        services.AddSingleton<AppProcessor>();
        services.AddSingleton<IDatabaseNode, DatabaseNode>();
        services.AddSingleton<IKeyValueStorage, KeyValueStorage>();
        services.AddSingleton<INodeRegistry, NodeRegistry>();
        services.AddSingleton<ICommandHandlerProvider, CommandHandlerProvider>();

        services.RegisterCommandHandlers();

        services.AddSingleton<Func<IEnumerable<CommandHandlerBase>>>(
            p => p.GetServices<CommandHandlerBase>);

        return services;
    }

    private static IServiceCollection RegisterCommandHandlers(this IServiceCollection services)
    {
        services.AddSingleton<CommandHandlerBase, SetValueCommandHandler>();
        services.AddSingleton<CommandHandlerBase, GetValueCommandHandler>();
        services.AddSingleton<CommandHandlerBase, FindKeyCommandHandler>();
        services.AddSingleton<CommandHandlerBase, GetMaxCommandHandler>();
        services.AddSingleton<CommandHandlerBase, GetMinCommandHandler>();
        services.AddSingleton<CommandHandlerBase, NewRecordCommandHandler>();
        services.AddSingleton<CommandHandlerBase, AddNodeCommandHandler>();
        services.AddSingleton<CommandHandlerBase, RemoveNodeCommandHandler>();
        services.AddSingleton<CommandHandlerBase, TerminateCommandHandler>();
        services.AddSingleton<CommandHandlerBase, ForwardCommandHandler>();

        return services;
    }
}
