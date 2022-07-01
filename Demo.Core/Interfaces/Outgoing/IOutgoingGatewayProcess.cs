namespace Demo.Core.Interfaces.Outgoing;

public interface IOutgoingGatewayProcess
{
    void Start(CancellationToken stoppingToken);
}