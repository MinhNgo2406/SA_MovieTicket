using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicket.Models;

namespace MovieTicket.Controllers
{
    public class AccountController : Controller
    {
       public MovieBookingContext ctx;
        public AccountController(MovieBookingContext _ctx)
        {
            ctx = _ctx;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = ctx.Users
                .FirstOrDefault(x => x.Email == username && x.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            // 🔐 Lưu session
            HttpContext.Session.SetString("Username", user.Name);
            HttpContext.Session.SetString("Role", user.Role);
            switch (user.Role)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user, string confirmPassword)
        {
            // kiểm tra confirm password
            if (user.Password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp";
                return View();
            }

            // kiểm tra email đã tồn tại chưa
            var checkEmail = ctx.Users.FirstOrDefault(x => x.Email == user.Email);

            if (checkEmail != null)
            {
                ViewBag.Error = "Email đã tồn tại";
                return View();
            }

            try
            {
                // mặc định role = User
                user.Role = "User";

                ctx.Users.Add(user);
                ctx.SaveChanges();

                TempData["Success"] = "Tạo tài khoản thành công 🎉";

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Tạo tài khoản thất bại";
                return View();
            }
        }
        public IActionResult Logout()
        {
            // xoá toàn bộ session
            HttpContext.Session.Clear();

            // quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
    }
}
