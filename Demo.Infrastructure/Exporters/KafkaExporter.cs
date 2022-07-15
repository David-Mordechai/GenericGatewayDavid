using Avro;
using Demo.Core.Interfaces;
using Demo.Core.Models;
using Demo.Infrastructure.Connectivity.MessageBrokers;

namespace Demo.Infrastructure.Exporters;

internal class KafkaExporter : IExporter
{
    private readonly IPublisher<GcsLightsAvro> _publisher;
    private Dictionary<string, string> _type2Topic = new();

    public KafkaExporter(IPublisher<GcsLightsAvro> publisher)
    {
        _publisher = publisher;
    }

    public void Init(ImporterExporter exporterSettings)
    {
        // Todo: maybe we should do generic list for all groups typeToTopic list
        // Todo: and remove this setting from group configuration.
        // Todo: then we can remove the Init method, it will simplify DI 
        _type2Topic = exporterSettings.TypeTopicMap;
    }

    public void Export(object export)
    {
        Export((MessageDto)export);
    }

    private async void Export(MessageDto messageDto)
    {
        if(messageDto.PayLoad is not GcsLightsAvro gcsLightsAvro) return;
        var topic = GetTopicByType(nameof(GcsLightsAvro));
        
        await _publisher.Publish(gcsLightsAvro, topic);
    }

    private string GetTopicByType(string type)
    {
        var topic = _type2Topic[type];
        return topic;
    }
}