using AeroCodeGenProtocols;
using Confluent.Kafka;
using Demo.Core.Interfaces;
using Demo.Core.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Importers;

public interface ITelemetryKafkaImporter
{
}

internal class TelemetryKafkaImporter : ITelemetryKafkaImporter, IImporter, IDisposable
{
    private readonly ILogger<TelemetryKafkaImporter> _logger;
    private Dictionary<string, string> _type2Topic = new();
    private IConsumer<Null, string>? _consumer;
    private ImporterExporter? _importerSettings;

    public event EventHandler<object>? DataReady;

    public TelemetryKafkaImporter(ILogger<TelemetryKafkaImporter> logger)
    {
        _logger = logger;
    }

    public void Init(ImporterExporter importerSettings)
    {
        _importerSettings = importerSettings;
        
        var ip = _importerSettings.Ip;
        var port = _importerSettings.Port;
        var clientId = _importerSettings.ClientId;
        _type2Topic = _importerSettings.TypeTopicMap;

        var config = new ConsumerConfig
        {
            BootstrapServers = $"{ip}:{port}",
            ClientId = clientId,
            GroupId = _importerSettings.ClientType
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    public void Start(CancellationToken cancellationToken)
    {
        var topic = GetTopicByType(nameof(GcsLightsRep));

        if (_consumer is null) return;

        _consumer.Subscribe(topic);
        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                DataReady?.Invoke(this, consumeResult.Message.Value);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case OperationCanceledException:
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        _consumer.Close();
                        break;
                    case ConsumeException exception:
                        _logger.LogError(ex, "Error message {errorMessage}", exception.Error.Reason);
                        break;
                    default:
                        _logger.LogError(ex, "Error message {errorMessage}", ex.Message);
                        break;
                }
            }
        }
    }

    private string GetTopicByType(string type)
    {
        var topic = _type2Topic[type];
        return topic;
    }

    public void Dispose()
    {
        _consumer?.Dispose();
    }
}