using Demo;
using Demo.Core.Models;
using Demo.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<Settings>(context.Configuration.GetSection(nameof(Settings)));
        var settings = context.Configuration.GetSection("Settings").Get<Settings>();
        services.AddSingleton(settings);

        services.RegisterGatewayServices(settings);

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
