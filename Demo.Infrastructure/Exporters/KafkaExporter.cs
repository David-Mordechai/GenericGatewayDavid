using Confluent.Kafka;
using Demo.Core.Interfaces;
using Demo.Core.Models;

namespace Demo.Infrastructure.Exporters;

internal class KafkaExporter : IExporter
{
    private readonly Dictionary<string, string> _type2Topic;
    private readonly IProducer<Null, string> _producer;

    public KafkaExporter(Settings settings)
    {
        var exporterSettings = settings.Exporter!;

        var ip = exporterSettings.Ip;
        var port = exporterSettings.Port;
        var clientId = exporterSettings.ClientId;
        _type2Topic = exporterSettings.TypeTopicMap;

        var config = new ProducerConfig
        {
            BootstrapServers = $"{ip}:{port}",
            ClientId = clientId,
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public void Export(object export)
    {
        Export((Message)export);
    }

    private async void Export(Message message)
    {
        var topic = GetTopicByType(message.Type);
        var value = GetValueByPayload(message.PayLoad);

        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = value });
    }

    private string GetTopicByType(string type)
    {
        var topic = _type2Topic[type];
        return topic;
    }

    private static string GetValueByPayload(object payload)
    {
        // TODO: handle this by settings
        return payload.ToString() ?? string.Empty;
    }
}