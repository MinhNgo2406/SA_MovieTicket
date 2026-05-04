using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.RabbitMQ;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentDbContext _ctx;

        public PaymentController(PaymentDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost("pay")]
        public IActionResult Pay(int orderId, decimal amount)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = "Momo",
                Status = "Success",
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now
            };

            _ctx.Payments.Add(payment);

            _ctx.SaveChanges();

            // publish event
            var publisher = new RabbitMQPublisher();

            publisher.Publish($"ORDER_PAID:{orderId}");

            return Ok(new
            {
                message = "Payment Success"
            });
        }
    }
}