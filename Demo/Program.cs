using Demo;
using Demo.Core.Models;
using Demo.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var settings = context.Configuration.GetSection(nameof(Settings)).Get<Settings>();
        services.AddSingleton(settings);

        services.RegisterGatewayServices(settings);

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
