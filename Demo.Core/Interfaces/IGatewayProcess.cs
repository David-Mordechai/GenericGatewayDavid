namespace Demo.Core.Interfaces;

public interface IGatewayProcess
{
    void Init(IImporter importer, IEnumerable<IProcessor> processors, IExporter exporter);
    void Start(CancellationToken stoppingToken);
}