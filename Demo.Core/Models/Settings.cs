﻿namespace Demo.Core.Models;

public class Settings
{
    public List<ImporterExporterGroup> ImporterExporterGroups { get; set; } = new();

    public IDictionary<string, (Type InterfaceType, Type ClassType)>
        InterfacesWithImplementationsDictionary { get; set; } =
        new Dictionary<string, (Type InterfaceType, Type ClassType)>();
}

public class ImporterExporterGroup
{
    public string? GroupName { get; set; }
    public bool IsActive { get; set; }
    public ImporterExporter? Importer { get; set; }
    public List<string> Processors { get; set; } = new();
    public ImporterExporter? Exporter { get; set; }
}

public class ImporterExporter
{
    public string Interface { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string Nic { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public Dictionary<string, string> TypeTopicMap { get; set; } = new();
}