using Demo.Core.Interfaces;
using Demo.Core.Models;
using Demo.Infrastructure.Exporters;
using Demo.Infrastructure.Importers;
using Demo.Infrastructure.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure;

public static class ServicesCollectionExtension
{
    public static void RegisterGatewayServices(this IServiceCollection services, Settings settings)
    {
        services.AddSingleton<IGatewayProcess, GatewayProcess>();
        RegisterImporter(services, settings.Importer);
        RegisterExporter(services, settings.Exporter);
        RegisterProcessors(services, settings.Processors);
    }

    private static void RegisterProcessors(IServiceCollection services, List<string> processors)
    {
        const string exceptionMessage = "Processors Implementations was not registered.";
        if (processors.Any() is false)
            throw new ArgumentException(exceptionMessage);

        foreach (var processor in processors)
        {
            switch (processor)
            {
                case "Telemetry2JsonProcessor":
                    services.AddSingleton<IProcessor, Telemetry2JsonProcessor>();
                    break;
                default:
                    throw new ArgumentException(exceptionMessage);
            }
        }
    }

    private static void RegisterExporter(IServiceCollection services, Settings.ImporterExporter? exporterSettings)
    {
        const string exceptionMessage = "Exporter Implementation was not registered.";
        if (exporterSettings == null)
            throw new ArgumentException(exceptionMessage);

        switch (exporterSettings.Class)
        {
            case "KafkaExporter":
                services.AddSingleton<IExporter, KafkaExporter>();
                break;
            default:
                throw new ArgumentException(exceptionMessage);
        }
    }

    private static void RegisterImporter(IServiceCollection services, Settings.ImporterExporter? importerSettings)
    {
        const string exceptionMessage = "Importer Implementation was not registered.";
        if(importerSettings == null) 
            throw new ArgumentException(exceptionMessage);
        
        switch (importerSettings.Class)
        {
            case "TelemetryImporter":
                services.AddSingleton<IImporter, TelemetryImporter>();
                break;
            default:
                throw new ArgumentException(exceptionMessage);
        }
    }
}