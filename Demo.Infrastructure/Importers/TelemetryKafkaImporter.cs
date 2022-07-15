using Avro;
using Demo.Core.Interfaces;
using Demo.Core.Models;
using Demo.Infrastructure.Connectivity.MessageBrokers;

namespace Demo.Infrastructure.Importers;

internal class TelemetryKafkaImporter : IImporter, IDisposable
{
    private readonly ISubscriber<GcsLightsAvro> _subscriber;
    private Dictionary<string, string> _type2Topic = new();

    public event EventHandler<object>? DataReady;

    public TelemetryKafkaImporter(ISubscriber<GcsLightsAvro> subscriber)
    {
        _subscriber = subscriber;
    }

    public void Init(ImporterExporter importerSettings)
    {
        _type2Topic = importerSettings.TypeTopicMap;
    }

    public void Start(CancellationToken cancellationToken)
    {
        var topic = GetTopicByType(nameof(GcsLightsAvro));

        _subscriber.Subscribe(topic, message =>
        {
            DataReady?.Invoke(this, message);
        }, cancellationToken);
        
    }

    private string GetTopicByType(string type)
    {
        var topic = _type2Topic[type];
        return topic;
    }

    public void Dispose()
    {
        _subscriber?.Dispose();
    }
}