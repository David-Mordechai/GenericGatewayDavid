using Demo.Core.Models;

namespace Demo.Core.Interfaces;

public interface IImporter
{
    void Init(ImporterExporter importerSettings);
    void Start(CancellationToken cancellationToken);
    
    event EventHandler<object> DataReady;
}