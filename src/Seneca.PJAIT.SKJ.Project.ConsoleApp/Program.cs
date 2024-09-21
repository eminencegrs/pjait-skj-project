using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seneca.PJAIT.SKJ.Project.ConsoleApp;
using Seneca.PJAIT.SKJ.Project.ConsoleApp.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(x => x.RegisterDependencies())
    .Build();

var appProcessor = host.Services.GetRequiredService<AppProcessor>();

await appProcessor.Run(args);
