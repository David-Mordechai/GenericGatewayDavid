using Demo.Core.Interfaces.Outgoing;

namespace Demo.Infrastructure.Outgoing;

internal class OutgoingGatewayProcess : IOutgoingGatewayProcess
{
    private readonly IOutgoingImporter _outgoingImporter;
    private readonly IEnumerable<IOutgoingProcessor> _outgoingProcessors;
    private readonly IOutgoingExporter _outgoingExporter;

    public OutgoingGatewayProcess(IOutgoingImporter outgoingImporter, 
        IEnumerable<IOutgoingProcessor> outgoingProcessors, IOutgoingExporter outgoingExporter)
    {
        _outgoingImporter = outgoingImporter;
        _outgoingProcessors = outgoingProcessors;
        _outgoingExporter = outgoingExporter;

        _outgoingImporter.DataReady += OutgoingImporterDataReady;
    }

    public void Start(CancellationToken stoppingToken)
    {
        _outgoingImporter.Start(stoppingToken);
    }

    private void OutgoingImporterDataReady(object? sender, object data)
    {
        foreach (var processor in _outgoingProcessors)
            data = processor.Process(data);

        _outgoingExporter.Export(data);
    }
}