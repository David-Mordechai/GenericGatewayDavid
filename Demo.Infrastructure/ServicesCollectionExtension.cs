using System.Reflection;
using Demo.Core.Interfaces;
using Demo.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure;

public static class ServicesCollectionExtension
{
    public static void RegisterGatewayServices(this IServiceCollection services, Settings settings)
    {
        services.AddSingleton<IGatewayProcess, GatewayProcess>();
        
        var assembly = Assembly.GetExecutingAssembly();
        RegisterImporter(services, settings.Importer, assembly);
        RegisterExporter(services, settings.Exporter, assembly);
        RegisterProcessors(services, settings.Processors, assembly);
    }

    private static void RegisterProcessors(IServiceCollection services, List<string> processors, Assembly assembly)
    {
        if (processors.Any() is false)
            throw new ArgumentException("Processors Implementations was not registered.");

        foreach (var processorType in processors.Select(className => GetType<IProcessor>(assembly, className)))
        {
            if (processorType is null)
                throw new ArgumentException($"Class {processors} was not registered.");

            services.AddSingleton(typeof(IProcessor), processorType);
        }
    }

    private static void RegisterExporter(IServiceCollection services, Settings.ImporterExporter? exporterSettings,
        Assembly assembly)
    {
        const string exceptionMessage = "Exporter Implementation was not registered.";
        if (exporterSettings is null)
            throw new ArgumentException(exceptionMessage);

        var exporterClassType = GetType<IExporter>(assembly, exporterSettings.Class);

        if (exporterClassType is null)
            throw new ArgumentException(exceptionMessage);

        services.AddSingleton(typeof(IExporter), exporterClassType);
    }

    private static void RegisterImporter(IServiceCollection services, Settings.ImporterExporter? importerSettings,
        Assembly assembly)
    {
        const string exceptionMessage = "Importer Implementation was not registered.";
        if(importerSettings is null) 
            throw new ArgumentException(exceptionMessage);
        
        var importerClassType = GetType<IImporter>(assembly, importerSettings.Class);
        
        if(importerClassType is null)
            throw new ArgumentException(exceptionMessage);

        services.AddSingleton(typeof(IImporter), importerClassType);
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