using Demo.Core.Interfaces;

namespace Demo.Core;

public class GatewayProcess : IGatewayProcess
{
    private IImporter? _importer;
    private IEnumerable<IProcessor> _processors = new List<IProcessor>();
    private IExporter? _exporter;

    public void Init(IImporter importer, IEnumerable<IProcessor> processors, IExporter exporter)
    {
        _importer = importer;
        _processors = processors;
        _exporter = exporter;

        _importer.DataReady += ImporterDataReady;
    }

    public void Start(CancellationToken stoppingToken)
    {
        _importer?.Start(stoppingToken);
    }

    private void ImporterDataReady(object? sender, object data)
    {
        foreach (var processor in _processors)
            data = processor.Process(data);

        _exporter?.Export(data);
    }
}