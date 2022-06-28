using Demo.Core.Interfaces;

namespace Demo;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IGatewayProcess _gatewayProcess;

    public Worker(ILogger<Worker> logger, IGatewayProcess gatewayProcess)
    {
        _logger = logger;
        _gatewayProcess = gatewayProcess;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Gateway process.");

        _gatewayProcess.Start(stoppingToken);
        
        return Task.CompletedTask;
    }
}