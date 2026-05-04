using Microsoft.AspNetCore.Mvc;
using MovieTicket.Models;

namespace MovieTicket.Controllers
{
    public class ProfileController : Controller
    {
        public MovieBookingContext ctx;
        public ProfileController(MovieBookingContext ctx)
        {
            this.ctx = ctx;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");

            var user = ctx.Users.FirstOrDefault(x => x.Name == username);

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(User model, string newPassword)
        {
            var user = ctx.Users.FirstOrDefault(x => x.Id == model.Id);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Name;
            user.Phone = model.Phone;

            // nếu có nhập password mới
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = newPassword;
            }

            ctx.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công 🎉";

            return RedirectToAction("Index");
        }
    }
}
