using AeroCodeGenProtocols;
using Confluent.Kafka;
using Demo.Core.Interfaces.Incoming;
using Demo.Core.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Incoming.Importers;

internal class KafkaImporter : IIncomingImporter, IDisposable
{
    private readonly ILogger<KafkaImporter> _logger;
    private readonly Dictionary<string, string> _type2Topic;
    private readonly IConsumer<Null, string> _consumer;

    public event EventHandler<object>? DataReady;

    public KafkaImporter(ILogger<KafkaImporter> logger, Settings settings)
    {
        _logger = logger;
        var exporterSettings = settings.OutgoingExporter!;

        var ip = exporterSettings.Ip;
        var port = exporterSettings.Port;
        var clientId = exporterSettings.ClientId;
        _type2Topic = exporterSettings.TypeTopicMap;

        var config = new ConsumerConfig
        {
            BootstrapServers = $"{ip}:{port}",
            ClientId = clientId,
            GroupId = exporterSettings.ClientType
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    public void Start(CancellationToken cancellationToken)
    {
        var topic = GetTopicByType(nameof(GcsLightsRep));

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
        _consumer.Dispose();
    }
}