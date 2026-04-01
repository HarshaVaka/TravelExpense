namespace UserService.Api.RabbitMq;

public class RabbitMqConfig
{
    public required string HostName { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Exchange { get; set; }

}