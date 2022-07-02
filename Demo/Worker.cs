using Demo.Core.Interfaces;
using Demo.Core.Models;
using Demo.Core;

namespace Demo;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Settings _settings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Worker(ILogger<Worker> logger, Settings settings, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _settings = settings;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Gateway processes.");
        var gatewayProcesses = new List<Task>();
        foreach (var settingsImporterExporterGroup in _settings.ImporterExporterGroups)
        {
            if(settingsImporterExporterGroup.IsActive is false) continue;
            
            var task = new Task(() =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var importerInterfaceName = settingsImporterExporterGroup.Importer!.Interface;
                var importerType = _settings.InterfacesWithImplementationsDictionary[importerInterfaceName].InterfaceType;
                if (scope.ServiceProvider.GetService(importerType) is not IImporter importer)
                    throw new ArgumentException($"{importerInterfaceName} was not resolved");
                importer.Init(settingsImporterExporterGroup.Importer);

                var exporterInterfaceName = settingsImporterExporterGroup.Exporter!.Interface;
                var exporterType = _settings.InterfacesWithImplementationsDictionary[exporterInterfaceName].InterfaceType;
                if (scope.ServiceProvider.GetService(exporterType) is not IExporter exporter)
                    throw new ArgumentException($"{exporterInterfaceName} was not resolved");
                exporter.Init(settingsImporterExporterGroup.Exporter);

                IList<IProcessor> processors = new List<IProcessor>();
                foreach (var processorInterfaceName in settingsImporterExporterGroup.Processors)
                {
                    var processorType = _settings.InterfacesWithImplementationsDictionary[processorInterfaceName].InterfaceType;

                    if (scope.ServiceProvider.GetService(processorType) is not IProcessor processor)
                        throw new ArgumentException($"{processorInterfaceName} was not resolved");

                    processors.Add(processor);
                }

                var gatewayProcess = scope.ServiceProvider.GetRequiredService<IGatewayProcess>();
                gatewayProcess.Init(importer, processors, exporter);
                gatewayProcess.Start(stoppingToken);

            }, stoppingToken);
            
            gatewayProcesses.Add(task);
        }

        gatewayProcesses.ForEach(x => x.Start());
        return Task.WhenAll(gatewayProcesses.ToArray());
    }
}