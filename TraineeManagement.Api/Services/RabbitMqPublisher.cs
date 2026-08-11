using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TraineeManagement.Shared.Configurations;
using TraineeManagement.Shared.Contracts;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Mime;

namespace TraineeManagement.Api.Services
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                ConnectionFactory factory = new()
                {
                    HostName = _options.Host,
                    Port = _options.Port,
                    VirtualHost = _options.VirtualHost,
                    UserName = _options.Username,
                    Password = _options.Password
                };

                await using IConnection connection = await factory.CreateConnectionAsync(cancellationToken);

                await using IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                Dictionary<string, object?> arguments = new()
                {
                    { "x-dead-letter-exchange", _options.DeadLetterExchange },
                    { "x-dead-letter-routing-key", _options.DeadLetterQueue }
                };
                await channel.QueueDeclareAsync(
                    queue: _options.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments,
                    cancellationToken: cancellationToken);

                byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                BasicProperties properties = new()
                {
                    Persistent = true,
                    ContentType = MediaTypeNames.Application.Json,
                    Type = typeof(TMessage).Name
                };

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: _options.QueueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("RabbitMQ message published successfully. MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                    typeof(TMessage).GetProperty("MessageId")?.GetValue(message),
                    typeof(TMessage).GetProperty("CorrelationId")?.GetValue(message));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish RabbitMQ message. MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                    typeof(TMessage).GetProperty("MessageId")?.GetValue(message),
                    typeof(TMessage).GetProperty("CorrelationId")?.GetValue(message));
                return false;
            }
        }
    }
}

