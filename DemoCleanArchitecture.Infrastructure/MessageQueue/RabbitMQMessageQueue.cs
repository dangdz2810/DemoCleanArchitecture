using DemoCleanArchitecture.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace DemoCleanArchitecture.Infrastructure.MessageQueue
{
    public class RabbitMQMessageQueue : IMessageQueue
    {
    //    private readonly ConnectionFactory _connectionFactory;
    //    private readonly string _queueName;
    //    private readonly ILogger<RabbitMQMessageQueue> _logger;
    //    public RabbitMQMessageQueue(string hostName, string queueName, ILogger<RabbitMQMessageQueue> logger)
    //    {
    //        _connectionFactory = new ConnectionFactory() { HostName = hostName };
    //        _queueName = queueName;
    //        _logger = logger;
    //    }

    //    public void Publish(string message)
    //    {
    //        using (var connection = _connectionFactory.CreateConnection())
    //        using (var channel = connection.CreateModel())
    //        {
    //            channel.QueueDeclare(queue: _queueName,
    //                                 durable: false,
    //                                 exclusive: false,
    //                                 autoDelete: false,
    //                                 arguments: null);

    //            var body = Encoding.UTF8.GetBytes(message);

    //            channel.BasicPublish(exchange: "",
    //                                 routingKey: _queueName,
    //                                 basicProperties: null,
    //                                 body: body);

    //            _logger.LogInformation($"[x] Sent '{message}'");
    //        }
    //    }

    //    public void Subscribe(Action<string> callback)
    //    {
    //        using (var connection = _connectionFactory.CreateConnection())
    //        using (var channel = connection.CreateModel())
    //        {
    //            channel.QueueDeclare(queue: _queueName,
    //                                 durable: false,
    //                                 exclusive: false,
    //                                 autoDelete: false,
    //                                 arguments: null);

    //            var consumer = new EventingBasicConsumer(channel);
    //            consumer.Received += (model, ea) =>
    //            {
    //                var body = ea.Body.ToArray();
    //                var message = Encoding.UTF8.GetString(body);
    //                callback(message);
    //            };

    //            channel.BasicConsume(queue: _queueName,
    //                                 autoAck: true,
    //                                 consumer: consumer);

    //            _logger.LogInformation($"[*] Waiting for messages in queue '{_queueName}'");
    //        }
    //    }
    }
}
