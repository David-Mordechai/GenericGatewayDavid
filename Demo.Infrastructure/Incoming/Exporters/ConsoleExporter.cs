using Demo.Core.Interfaces.Incoming;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Incoming.Exporters;

internal class ConsoleExporter : IIncomingExporter
{
    private readonly ILogger<ConsoleExporter> _logger;

    public ConsoleExporter(ILogger<ConsoleExporter> logger)
    {
        _logger = logger;
    }
    public void Export(object export)
    {
        _logger.LogInformation((string)export);
    }
}