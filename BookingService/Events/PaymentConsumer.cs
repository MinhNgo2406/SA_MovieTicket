using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using BookingService.Events;
using BookingService.Data;

namespace BookingService.Services
{
    public class PaymentConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentConsumer(
            IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq"
            };

            var connection =
                factory.CreateConnection();

            var channel =
                connection.CreateModel();

            channel.QueueDeclare(
                queue: "payment-success",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer =
                new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var json =
                    Encoding.UTF8.GetString(body);

                var paymentEvent =
                    JsonSerializer.Deserialize
                    <PaymentSuccessEvent>(json);

                using var scope =
                    _scopeFactory.CreateScope();

                var db =
                    scope.ServiceProvider
                    .GetRequiredService<BookingDbContext>();

                var order =
                    db.Orders
                    .FirstOrDefault(x =>
                        x.Id == paymentEvent.OrderId);

                if (order != null)
                {
                    order.Status = "Paid";

                    db.SaveChanges();
                }

                Console.WriteLine(
                    $"Payment Success: {json}");
            };

            channel.BasicConsume(
                queue: "payment-success",
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }
    }
}