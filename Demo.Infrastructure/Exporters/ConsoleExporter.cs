using Demo.Core.Interfaces;
using Demo.Core.Models;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Exporters;

public interface ILoggerExporter { }

internal class LoggerExporter : ILoggerExporter, IExporter
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
        _logger.LogInformation((string)export);
    }
}