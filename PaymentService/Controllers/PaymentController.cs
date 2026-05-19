using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Models;
using PaymentService.RabbitMQ;
using System.Text.Json;
using PaymentService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        public IActionResult Pay([FromBody] PaymentRequest request)
        {
            var payment = new Payment
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                PaymentMethod = "Momo",
                Status = "Success",
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now
            };

            _ctx.Payments.Add(payment);

            _ctx.SaveChanges();

            // publish event
            var publisher = new RabbitMQPublisher();

            var paymentEvent = new
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                TransactionId = payment.TransactionId
            };

            publisher.Publish(JsonSerializer.Serialize(paymentEvent));

            return Ok(new
            {
                message = "Payment Success"
            });
        }
        [HttpGet]
        public IActionResult GetPayments()
        {
            var payments = _ctx.Payments.ToList();

            return Ok(payments);
        }

        [HttpGet("{id}")]
        public IActionResult GetPayment(int id)
        {
            var payment = _ctx.Payments.Find(id);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        
    }
}