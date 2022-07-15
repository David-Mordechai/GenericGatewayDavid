using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Demo.Core.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Connectivity.MessageBrokers.Kafka;

internal class KafkaPublisherAdapter<T> : IPublisher<T> where T : class
{
    private readonly ILogger<KafkaPublisherAdapter<T>> _logger;
    private readonly IProducer<string, T> _producer;
    private readonly CachedSchemaRegistryClient _schemaRegistry;

    public KafkaPublisherAdapter(ILogger<KafkaPublisherAdapter<T>> logger, Settings settings)
    {
        _logger = logger;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = settings.SchemaRegistryUrl
        };

        var avroSerializerConfig = new AvroSerializerConfig
        {
            BufferBytes = 100
        };
       
        try
        {
            _schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            _producer = new ProducerBuilder<string, T>(producerConfig)
                .SetValueSerializer(new AvroSerializer<T>(_schemaRegistry, avroSerializerConfig))
                .Build();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Kafka Producer failed to open connection.");
            throw;
        }
        
    }

    public async Task Publish(T message, string topic) 
    {
        try
        {
            await _producer
                .ProduceAsync(topic, new Message<string, T> {Key = string.Empty, Value = message});
        }
        catch (Exception)
        {
            _logger.LogWarning("Kafka producer failed to publish message to topic: {Topic}.", topic);
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
        _schemaRegistry.Dispose();
    }
}