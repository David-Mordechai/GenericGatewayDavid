namespace Demo.Core;

public interface IImporter
{
    void Start();
    void Stop();
    event EventHandler<object> DataReady;
}