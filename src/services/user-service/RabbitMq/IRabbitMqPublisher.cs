namespace UserService.Api.RabbitMq;

public interface IRabbitMqPublisher
{
    void PublishUserRegistered(object message);
}