using AspNetCoreHero.ToastNotification.Abstractions;
using MichiBlog.Models;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MichiBlog.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;
        public UserController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                INotyfService notification) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notification;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userVMs = users.Select(x => new UserVM() 
            { 
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
            }).ToList();

            return View(userVMs);
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated) {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) {
                return View(login);
            }
            var existingUser = _userManager.Users.FirstOrDefault(x => x.UserName == login.Username);
            if (existingUser == null) {
                _notification.Error("Username does not exist");
                return View(login);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, login.Password ?? string.Empty);
            if (!verifyPassword)
            { 
                _notification.Error("Password does not match");
                return View(login);
            }
            await _signInManager.PasswordSignInAsync(existingUser, login.Password ?? string.Empty, login.RememberMe, true);
            _notification.Success("Login Successful");
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notification.Success("Logout Successful");
            return RedirectToAction("Login", "User", new { area = "Admin" });
        }
    }
}
