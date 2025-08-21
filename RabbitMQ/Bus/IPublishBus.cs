using MassTransit;

namespace RabbitMQ.Bus;

public interface IPublishBus 
{
    Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class;
}

public class PublishBus : IPublishBus 
{
    private readonly IPublishEndpoint _busEndPoint;

    public PublishBus(IPublishEndpoint publish)
    {
        _busEndPoint = publish;
    }

    public Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
    {
        return _busEndPoint.Publish(message, ct);
    }
}