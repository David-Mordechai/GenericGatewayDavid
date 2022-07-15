using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Demo.Core.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Connectivity.MessageBrokers.Kafka;

internal class KafkaConsumerAdapter<T> : ISubscriber<T> where T : class
{
    private readonly ILogger<KafkaConsumerAdapter<T>> _logger;
    private readonly IConsumer<string, T> _consumer;
    private readonly CachedSchemaRegistryClient _schemaRegistry;

    public KafkaConsumerAdapter(ILogger<KafkaConsumerAdapter<T>> logger, Settings settings)
    {
        _logger = logger;
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            GroupId = "Aero"
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = settings.SchemaRegistryUrl
        };

        try
        {
            _schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            _consumer = new ConsumerBuilder<string, T>(consumerConfig)
                .SetValueDeserializer(new AvroDeserializer<T>(_schemaRegistry).AsSyncOverAsync())
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .Build();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Kafka Consumer failed to open connection.");
            throw;
        }
        
    }

    public void Subscribe(string topic, Action<T> consumeMessageHandler, CancellationToken cancellationToken)
    {
        _consumer.Subscribe(topic);

        while (cancellationToken.IsCancellationRequested is false)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                consumeMessageHandler.Invoke(consumeResult.Message.Value);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case OperationCanceledException:
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        _consumer.Close();
                        throw new Exception("Operation was canceled.");
                    case ConsumeException:
                        _logger.LogWarning(
                            "Kafka Consumer failed to consume message from topic: {Topic}. Reason: {ConsumeFailReason}",
                            topic, ex.Message);
                        break;
                    default:
                        _logger.LogError(ex, "Kafka Consumer subscribe method failed.");
                        throw new Exception(ex.Message);
                }
            }
        }
    }

    public void Dispose()
    {
        _consumer.Dispose();
        _schemaRegistry.Dispose();
    }
}