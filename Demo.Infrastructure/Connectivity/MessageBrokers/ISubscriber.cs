namespace Demo.Infrastructure.Connectivity.MessageBrokers;

internal interface ISubscriber<out T> : IDisposable where T : class
{
    void Subscribe(string topic, Action<T> consumeMessageHandler, CancellationToken cancellationToken);
}