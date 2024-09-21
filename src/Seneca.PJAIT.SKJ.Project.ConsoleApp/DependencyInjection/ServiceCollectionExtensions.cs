using Microsoft.Extensions.DependencyInjection;
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

        return services;
    }
}
