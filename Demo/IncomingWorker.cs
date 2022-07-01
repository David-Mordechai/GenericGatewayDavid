using Demo.Core.Interfaces.Incoming;

namespace Demo;

public class IncomingWorker : BackgroundService
{
    private readonly ILogger<IncomingWorker> _logger;
    private readonly IIncomingGatewayProcess _incomingGatewayProcess;

    public IncomingWorker(ILogger<IncomingWorker> logger, IIncomingGatewayProcess incomingGatewayProcess)
    {
        _logger = logger;
        _incomingGatewayProcess = incomingGatewayProcess;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Incoming Gateway process.");

        _incomingGatewayProcess.Start(stoppingToken);

        return Task.CompletedTask;
    }
}