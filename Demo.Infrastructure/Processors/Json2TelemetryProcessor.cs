using Demo.Core.Interfaces;

namespace Demo.Infrastructure.Processors;

public interface IJson2TelemetryProcessor { }

internal class Json2TelemetryProcessor : IJson2TelemetryProcessor, IProcessor
{
    public object Process(object obj)
    {
        return obj;
    }
}