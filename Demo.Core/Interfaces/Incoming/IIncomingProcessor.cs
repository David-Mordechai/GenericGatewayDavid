namespace Demo.Core.Interfaces.Incoming;

public interface IIncomingProcessor
{
    object Process(object obj);
}