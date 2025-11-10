using OrderProcessing.Contracts;

namespace OrderProcessing.Infrastructure.Interfaces;

public interface IRabbitMqPublisher
{
    Task PublishAsync(OrderSubmittedMessage message);
}