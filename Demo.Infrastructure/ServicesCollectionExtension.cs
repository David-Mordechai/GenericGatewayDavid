using System.Reflection;
using Demo.Core;
using Demo.Core.Interfaces;
using Demo.Core.Models;
using Demo.Infrastructure.Connectivity.MessageBrokers;
using Demo.Infrastructure.Connectivity.MessageBrokers.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure;

public static class ServicesCollectionExtension
{
    public static void RegisterGatewayServices(this IServiceCollection services, Settings settings)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var classesTypesDictionary = new Dictionary<string, Type>();
        
        services.AddScoped<IGatewayProcess, GatewayProcess>();
        services.AddSingleton(typeof(IPublisher<>), typeof(KafkaPublisherAdapter<>));
        services.AddSingleton(typeof(ISubscriber<>), typeof(KafkaConsumerAdapter<>));

        foreach (var group in settings.ImporterExporterGroups)
        {
            RegisterImporter(services, group.Importer, assembly, classesTypesDictionary);
            RegisterExporter(services, group.Exporter, assembly, classesTypesDictionary);
            RegisterProcessors(services, group.Processors, assembly, classesTypesDictionary);
        }

        settings.ClassesTypesDictionary = classesTypesDictionary;
    }

    private static void RegisterImporter(IServiceCollection services, ImporterExporter? importerSettings,
        Assembly assembly, IDictionary<string, Type> dictionary)
    {
        const string exceptionMessage = "Importer Implementation was not registered.";
        if (importerSettings is null)
            throw new ArgumentException(exceptionMessage);

        var importerClassType = GetType<IImporter>(assembly, importerSettings.Class);

        if (importerClassType is null)
            throw new ArgumentException(exceptionMessage);

        if (dictionary.ContainsKey(importerSettings.Class)) return;

        dictionary.Add(importerSettings.Class, importerClassType);
        services.AddScoped(importerClassType);
    }

    private static void RegisterExporter(IServiceCollection services, ImporterExporter? exporterSettings,
        Assembly assembly, IDictionary<string, Type> dictionary)
    {
        const string exceptionMessage = "IExporter Implementation was not registered.";
        if (exporterSettings is null)
            throw new ArgumentException(exceptionMessage);

        var exporterClassType = GetType<IExporter>(assembly, exporterSettings.Class);

        if (exporterClassType is null)
            throw new ArgumentException(exceptionMessage);

        if (dictionary.ContainsKey(exporterSettings.Class)) return;

        dictionary.Add(exporterSettings.Class, exporterClassType);
        services.AddScoped(exporterClassType);
    }
    
    private static void RegisterProcessors(IServiceCollection services, List<string>? processors,
        Assembly assembly, IDictionary<string, Type> dictionary)
    {
        if (processors is null || processors.Any() is false)
            throw new ArgumentException("Processors Implementations was not registered.");

        foreach (var processor in processors)
        {
            var processorType = GetType<IProcessor>(assembly, processor);

            if (processorType is null)
                throw new ArgumentException($"Class {processor} that implement IProcessor was not found");

            if (dictionary.ContainsKey(processor)) return;

            dictionary.Add(processor, processorType);
            services.AddScoped(processorType);
        }
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