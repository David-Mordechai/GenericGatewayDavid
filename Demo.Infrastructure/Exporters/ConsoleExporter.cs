using System.Text.Json;
using Demo.Core.Interfaces;
using Demo.Core.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Exporters;

internal class LoggerExporter : IExporter
{
    private readonly ILogger<LoggerExporter> _logger;

    public LoggerExporter(ILogger<LoggerExporter> logger)
    {
        _logger = logger;
    }

    public void Init(ImporterExporter exporterSettings)
    {
       
    }

    public void Export(object export)
    {
        var jsonResult = JsonSerializer.Serialize(export);
        _logger.LogInformation(jsonResult);
    }
}