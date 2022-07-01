namespace Demo.Core.Interfaces.Outgoing;

public interface IOutgoingImporter
{
    void Start(CancellationToken cancellationToken);

    event EventHandler<object> DataReady;
}