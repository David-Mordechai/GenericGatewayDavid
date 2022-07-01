using Demo.Core.Interfaces.Incoming;

namespace Demo.Infrastructure.Incoming.Processors;

internal class Json2TelemetryProcessor : IIncomingProcessor
{
    public object Process(object obj)
    {
        return obj;
    }
}