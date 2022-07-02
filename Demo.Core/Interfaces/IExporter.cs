using Demo.Core.Models;

namespace Demo.Core.Interfaces;

public interface IExporter
{
    void Init(ImporterExporter exporterSettings);
    void Export(object export);
}