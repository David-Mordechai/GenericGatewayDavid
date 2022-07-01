namespace Demo.Core.Models;

public class Settings
{
    public ImporterExporter? OutgoingImporter { get; set; }
    public List<string> OutgoingProcessors { get; set; } = new();
    public ImporterExporter? OutgoingExporter { get; set; }

    public ImporterExporter? IncomingImporter { get; set; }
    public List<string> IncomingProcessors { get; set; } = new();
    public ImporterExporter? IncomingExporter { get; set; }

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