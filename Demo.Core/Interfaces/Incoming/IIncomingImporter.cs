namespace Demo.Core.Interfaces.Incoming;

public interface IIncomingImporter
{
    void Start(CancellationToken cancellationToken);

    event EventHandler<object> DataReady;
}