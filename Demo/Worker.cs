using Demo.Core.Interfaces;
using Demo.Core.Models;

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
        foreach (var group in _settings.ImporterExporterGroups.Where(x => x.IsActive))
        {
            var task = new Task(() =>
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var importerClassName = group.Importer!.Class;
                var importerType = _settings.ClassesTypesDictionary[importerClassName];
                if (scope.ServiceProvider.GetService(importerType) is not IImporter importer)
                    throw new ArgumentException($"{importerClassName} was not resolved");
                importer.Init(group.Importer);

                var exporterClassName = group.Exporter!.Class;
                var exporterType = _settings.ClassesTypesDictionary[exporterClassName];
                if (scope.ServiceProvider.GetService(exporterType) is not IExporter exporter)
                    throw new ArgumentException($"{exporterClassName} was not resolved");
                exporter.Init(group.Exporter);

                IList<IProcessor> processors = new List<IProcessor>();
                foreach (var processorClassName in group.Processors)
                {
                    var processorType = _settings.ClassesTypesDictionary[processorClassName];

                    if (scope.ServiceProvider.GetService(processorType) is not IProcessor processor)
                        throw new ArgumentException($"{processorClassName} was not resolved");

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