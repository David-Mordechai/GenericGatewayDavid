namespace Demo.Core.Interfaces;

public interface IImporter
{
    void Start(CancellationToken cancellationToken);

    event EventHandler<object> DataReady;
}