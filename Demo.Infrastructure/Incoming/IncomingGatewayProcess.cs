using Demo.Core.Interfaces.Incoming;

namespace Demo.Infrastructure.Incoming;

internal class IncomingGatewayProcess : IIncomingGatewayProcess
{
    private readonly IIncomingImporter _incomingImporter;
    private readonly IEnumerable<IIncomingProcessor> _incomingProcessors;
    private readonly IIncomingExporter _incomingExporter;

    public IncomingGatewayProcess(IIncomingImporter incomingImporter,
        IEnumerable<IIncomingProcessor> incomingProcessors, IIncomingExporter incomingExporter)
    {
        _incomingImporter = incomingImporter;
        _incomingProcessors = incomingProcessors;
        _incomingExporter = incomingExporter;

        _incomingImporter.DataReady += IncomingImporterDataReady;
    }

    public void Start(CancellationToken stoppingToken)
    {
        _incomingImporter.Start(stoppingToken);
    }

    private void IncomingImporterDataReady(object? sender, object data)
    {
        foreach (var processor in _incomingProcessors)
            data = processor.Process(data);

        _incomingExporter.Export(data);
    }
}