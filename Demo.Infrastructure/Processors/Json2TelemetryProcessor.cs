using Demo.Core.Interfaces;

namespace Demo.Infrastructure.Processors;

internal class Json2TelemetryProcessor : IProcessor
{
    public object Process(object obj)
    {
        return obj;
    }
}