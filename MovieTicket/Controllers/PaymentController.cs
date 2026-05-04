using Microsoft.AspNetCore.Mvc;
using MovieTicket.Models;

namespace MovieTicket.Controllers
{
    public class PaymentController : Controller
    {
        private readonly MovieBookingContext _ctx;

        public PaymentController(MovieBookingContext ctx)
        {
            _ctx = ctx;
        }

        // checkout page
        public IActionResult Checkout(int orderId)
        {
            var order = _ctx.Orders
                .FirstOrDefault(x => x.Id == orderId);

            if (order == null)
            {
                return Content("ORDER NULL");
            }

            return View(order);
        }

        // pay
        [HttpPost]
        public IActionResult Pay(int orderId)
        {
            var order = _ctx.Orders
                .FirstOrDefault(x => x.Id == orderId);

            if (order == null)
                return NotFound();

            // create payment
            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.TotalAmount,
                PaymentMethod = "Momo",
                TransactionId = Guid.NewGuid().ToString(),
                Status = "Success",
                PaymentDate = DateTime.Now
            };

            _ctx.Payments.Add(payment);

            // update order
            order.Status = "Paid";

            _ctx.SaveChanges();

            return RedirectToAction(
                "Success",
                new { orderId = order.Id });
        }

        public IActionResult Success(int orderId)
        {
            var order = _ctx.Orders
                .FirstOrDefault(x => x.Id == orderId);

            return View(order);
        }
    }
}