using System.Reflection;
using Demo.Core.Interfaces.Incoming;
using Demo.Core.Interfaces.Outgoing;
using Demo.Core.Models;
using Demo.Infrastructure.Incoming;
using Demo.Infrastructure.Outgoing;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure;

public static class ServicesCollectionExtension
{
    public static void RegisterGatewayServices(this IServiceCollection services, Settings settings)
    {
        services.AddSingleton<IOutgoingGatewayProcess, OutgoingGatewayProcess>();
        services.AddSingleton<IIncomingGatewayProcess, IncomingGatewayProcess>();
        
        var assembly = Assembly.GetExecutingAssembly();
        
        RegisterImporter<IOutgoingImporter>(services, settings.OutgoingImporter, assembly);
        RegisterExporter<IOutgoingExporter>(services, settings.OutgoingExporter, assembly);
        RegisterProcessors<IOutgoingProcessor>(services, settings.OutgoingProcessors, assembly);

        RegisterImporter<IIncomingImporter>(services, settings.IncomingImporter, assembly);
        RegisterExporter<IIncomingExporter>(services, settings.IncomingExporter, assembly);
        RegisterProcessors<IIncomingProcessor>(services, settings.IncomingProcessors, assembly);
    }

    private static void RegisterProcessors<T>(IServiceCollection services, List<string> processors, Assembly assembly)
    {
        if (processors.Any() is false)
            throw new ArgumentException("Processors Implementations was not registered.");

        foreach (var processorType in processors.Select(className => GetType<T>(assembly, className)))
        {
            if (processorType is null)
                throw new ArgumentException($"Processor {processors} Implementation was not registered.");

            services.AddSingleton(typeof(T), processorType);
        }
    }

    private static void RegisterExporter<T>(IServiceCollection services, Settings.ImporterExporter? exporterSettings,
        Assembly assembly)
    {
        const string exceptionMessage = "Exporter Implementation was not registered.";
        if (exporterSettings is null)
            throw new ArgumentException(exceptionMessage);

        var exporterClassType = GetType<T>(assembly, exporterSettings.Class);

        if (exporterClassType is null)
            throw new ArgumentException(exceptionMessage);

        services.AddSingleton(typeof(T), exporterClassType);
    }

    private static void RegisterImporter<T>(IServiceCollection services, Settings.ImporterExporter? importerSettings,
        Assembly assembly)
    {
        const string exceptionMessage = "Importer Implementation was not registered.";
        if(importerSettings is null) 
            throw new ArgumentException(exceptionMessage);
        
        var importerClassType = GetType<T>(assembly, importerSettings.Class);
        
        if(importerClassType is null)
            throw new ArgumentException(exceptionMessage);

        services.AddSingleton(typeof(T), importerClassType);
    }

    private static Type? GetType<T>(Assembly assembly, string className)
    {
        var info = typeof(T).GetTypeInfo();
        var result =
            info.Assembly
                .GetTypes()
                .Concat(assembly.GetTypes())
                .Where(x => info.IsAssignableFrom(x))
                .Where(x => x.IsClass && !x.IsAbstract)
                .FirstOrDefault(x => x.Name == className);

        return result;
    }
}