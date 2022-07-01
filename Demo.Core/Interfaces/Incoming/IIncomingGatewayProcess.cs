namespace Demo.Core.Interfaces.Incoming;

public interface IIncomingGatewayProcess
{
    void Start(CancellationToken stoppingToken);
}