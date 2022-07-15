namespace Demo.Infrastructure.Connectivity.MessageBrokers;

internal interface IPublisher<in T> : IDisposable where T : class
{
    Task Publish(T message, string topic);
}