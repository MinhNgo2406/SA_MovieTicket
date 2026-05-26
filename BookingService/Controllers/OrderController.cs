using Microsoft.AspNetCore.Mvc;
using BookingService.Data;
using BookingService.Models;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly BookingDbContext _context;

        public OrderController(BookingDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}