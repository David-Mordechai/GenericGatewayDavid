using Demo.Core.Interfaces;

namespace Demo.Infrastructure;

internal class GatewayProcess : IGatewayProcess
{
    private readonly IImporter _importer;
    private readonly IEnumerable<IProcessor> _processors;
    private readonly IExporter _exporter;

    public GatewayProcess(IImporter importer, IEnumerable<IProcessor> processors, IExporter exporter)
    {
        _importer = importer;
        _processors = processors;
        _exporter = exporter;

        _importer.DataReady += Importer_DataReady;
    }

    public void Start(CancellationToken stoppingToken)
    {
        _importer.Start(stoppingToken);
    }

    private void Importer_DataReady(object? sender, object data)
    {
        foreach (var processor in _processors)
            data = processor.Process(data);

        _exporter.Export(data);
    }
}