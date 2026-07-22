using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Shared.Configurations;
using TraineeManagement.Shared.Contracts;
using TraineeManagement.Shared.Models;
using TraineeManagement.Shared.Data;
using TraineeManagement.Shared.Enums;
using TraineeManagement.Worker.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqOptions _options;

        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public Worker(ILogger<Worker> logger, IOptions<RabbitMqOptions> options, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _options = options.Value;
            _scopeFactory = scopeFactory;
        }
        private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
        {
            ConnectionFactory factory = new()
            {
                HostName = _options.Host,
                Port = _options.Port,
                VirtualHost = _options.VirtualHost,
                UserName = _options.Username,
                Password = _options.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);

            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(
                    exchange: _options.DeadLetterExchange,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken
            );

            Dictionary<string, object?> arguments = new()
            {
                { "x-dead-letter-exchange", _options.DeadLetterExchange },
                { "x-dead-letter-routing-key", _options.DeadLetterQueue }
            };

            await _channel.QueueDeclareAsync(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments,
                cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _options.DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await _channel.QueueBindAsync(
                queue: _options.DeadLetterQueue,
                exchange: _options.DeadLetterExchange,
                routingKey: _options.DeadLetterQueue,
                cancellationToken: cancellationToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ connected successfully.");
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await InitializeRabbitMqAsync(cancellationToken);
            _logger.LogInformation("RabbitMQ Started");

            AsyncEventingBasicConsumer consumer = new(_channel!);
            _logger.LogInformation("Consumer Created");


            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                _logger.LogInformation("Message Received");
                try
                {
                    string json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

                    SubmissionProcessingRequested? message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);

                    if (message is null)
                    {
                        _logger.LogWarning("Received an invalid message.");
                        await _channel!.BasicRejectAsync(
                            eventArgs.DeliveryTag,
                            requeue: false,
                            cancellationToken: cancellationToken);

                        return;
                    }

                    using IServiceScope scope = _scopeFactory.CreateScope();
                    ISubmissionProcessorService processor = scope.ServiceProvider.GetRequiredService<ISubmissionProcessorService>();
                    ProcessingResultStatus result = await processor.ProcessAsync(message, cancellationToken);

                    switch (result)
                    {
                        case ProcessingResultStatus.Success:

                            await _channel!.BasicAckAsync(
                                eventArgs.DeliveryTag,
                                false,
                                cancellationToken);

                            break;

                        case ProcessingResultStatus.Retry:

                            await _channel!.BasicNackAsync(
                                eventArgs.DeliveryTag,
                                false,
                                true,
                                cancellationToken);

                            break;

                        case ProcessingResultStatus.DeadLetter:

                            await _channel!.BasicRejectAsync(
                                eventArgs.DeliveryTag,
                                false,
                                cancellationToken);

                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected worker error.");

                    await _channel!.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        false,
                        true,
                        cancellationToken);
                }
            };

            await _channel!.BasicConsumeAsync(
                queue: _options.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);

            _logger.LogInformation("RabbitMQ consumer started.");

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null)
                await _channel.DisposeAsync();

            if (_connection is not null)
                await _connection.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}

