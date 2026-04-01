using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace UserService.Api.RabbitMq;

public class RabbitMqPublisher(IConnection connection, IOptions<RabbitMqConfig> config) : IRabbitMqPublisher
{
    private readonly RabbitMqConfig _config = config.Value;

    public void PublishUserRegistered(object message)
    {
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(_config.Exchange, ExchangeType.Direct);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: _config.Exchange,
            routingKey: "user.registered",
            basicProperties: null,
            body: body
        );
    }
}