using Demo.Core.Interfaces.Outgoing;

namespace Demo;

public class OutgoingWorker : BackgroundService
{
    private readonly ILogger<OutgoingWorker> _logger;
    private readonly IOutgoingGatewayProcess _outgoingGatewayProcess;

    public OutgoingWorker(ILogger<OutgoingWorker> logger, IOutgoingGatewayProcess outgoingGatewayProcess)
    {
        _logger = logger;
        _outgoingGatewayProcess = outgoingGatewayProcess;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Outgoing Gateway process.");

        _outgoingGatewayProcess.Start(stoppingToken);
        
        return Task.CompletedTask;
    }
}