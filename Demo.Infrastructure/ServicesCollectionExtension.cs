using System.Reflection;
using Demo.Core;
using Demo.Core.Interfaces;
using Demo.Core.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure;

public static class ServicesCollectionExtension
{
    public static void RegisterGatewayServices(this IServiceCollection services, Settings settings)
    {
        settings.InterfacesWithImplementationsDictionary = BuildDictionaryOfInterfacesWithImplementations();
        
        services.AddScoped<IGatewayProcess, GatewayProcess>();

        foreach (var group in settings.ImporterExporterGroups)
        {
            RegisterImporter(services, group.Importer, settings);
            RegisterExporter(services, group.Exporter, settings);
            RegisterProcessors(services, group.Processors, settings);
        }
    }

    private static void RegisterProcessors(IServiceCollection services, List<string>? processors, Settings settings)
    {
        if (processors is null || processors.Any() is false)
            throw new ArgumentException("Processors Implementations was not registered.");

        foreach (var processor in processors)
        {
            if (processor is null)
                throw new ArgumentException($"Processor {processors} Implementation was not registered.");

            Register(services, settings, processor);
        }
    }

    private static void RegisterExporter(IServiceCollection services, ImporterExporter? exporterSettings, Settings settings)
    {
        const string exceptionMessage = "IExporter Implementation was not registered.";
        if (exporterSettings is null)
            throw new ArgumentException(exceptionMessage);

        Register(services, settings, exporterSettings.Interface);
    }
    
    private static void RegisterImporter(IServiceCollection services, ImporterExporter? importerSettings, Settings settings)
    {
        const string exceptionMessage = "Importer Implementation was not registered.";
        if(importerSettings is null) 
            throw new ArgumentException(exceptionMessage);

        Register(services, settings, importerSettings.Interface);
    }

    private static void Register(IServiceCollection services, Settings settings, string processor)
    {
        var notExist = settings.InterfacesWithImplementationsDictionary.ContainsKey(processor) is false;
        if (notExist)
            throw new ArgumentException($"{processor} interface or implementation class not found");

        var (@interface, @class) = settings.InterfacesWithImplementationsDictionary[processor];

        services.AddScoped(@interface, @class);
    }

    private static IDictionary<string, (Type InterfaceType, Type ClassType)> 
        BuildDictionaryOfInterfacesWithImplementations()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var dictionary = new Dictionary<string, (Type InterfaceType, Type ClassType)>();
        var interfaces = assembly.GetTypes().Where(x => x.IsInterface).ToList();
        foreach (var @interface in interfaces)
        {
            var @class = GetClassByInterface(assembly, @interface);
            
            if(@class is null)
                throw new ArgumentException($"{@interface.Name} has no implementation class");

            if (dictionary.ContainsKey(@interface.Name))
                throw new ArgumentException($"{@interface.Name} has more than one implementation class");

            dictionary.Add(@interface.Name,(@interface, @class));
        }
        return dictionary;
    }

    private static Type? GetClassByInterface(Assembly assembly, Type @interface)
    {
        var info = @interface.GetTypeInfo();
        var result =
            info.Assembly
                .GetTypes()
                .Concat(assembly.GetTypes())
                .Where(x => info.IsAssignableFrom(x))
                .FirstOrDefault(x => x.IsClass && !x.IsAbstract);

        return result;
    }
}