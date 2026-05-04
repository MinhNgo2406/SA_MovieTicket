using RabbitMQ.Client;
using System.Text;

namespace PaymentService.RabbitMQ
{
    public class RabbitMQPublisher
    {
        public void Publish(string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "payment_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: "payment_queue",
                basicProperties: null,
                body: body);
        }
    }
}