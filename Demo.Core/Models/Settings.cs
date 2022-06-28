namespace Demo.Core.Models;

public class Settings
{
    public ImporterExporter? Importer { get; set; }
    public List<string> Processors { get; set; } = new();
    public ImporterExporter? Exporter { get; set; }

    public class ImporterExporter
    {
        public string Class { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public string Nic { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientType { get; set; } = string.Empty;
        public Dictionary<string, string> TypeTopicMap { get; set; } = new();
    }
}