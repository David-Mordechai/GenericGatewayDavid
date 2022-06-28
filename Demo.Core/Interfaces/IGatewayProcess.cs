namespace Demo.Core.Interfaces;

public interface IGatewayProcess
{
    void Start(CancellationToken stoppingToken);
}