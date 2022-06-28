namespace Demo.Core.Records;

public class ImporterExporter
{
    public string Class { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string Nic { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public KeyValuePair<string, string> TypeTopicMap { get; set; }
}

public class Settings
{
    public ImporterExporter? Importer { get; set; }
    public List<IProcessor> Processors { get; set; } = new();
    public ImporterExporter? Exporter { get; set; }
}