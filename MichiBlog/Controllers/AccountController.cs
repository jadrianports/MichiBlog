using AspNetCoreHero.ToastNotification.Abstractions;
using MichiBlog.Models;
using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.Interfaces;
using MichiBlog.WebApp.Models;
using MichiBlog.WebApp.Services;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MichiBlog.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IEmailService _emailService;
        private readonly ICaptchaService _captchaService;

        public AccountController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                ApplicationDbContext context,
                                INotyfService notification,
                                IEmailService emailService,
                                ICaptchaService captchaService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _notification = notification;
            _emailService = emailService;
            _captchaService = captchaService;
        }
        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View(new LoginVM());
            }
            _notification.Error("You are already logged in.");
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM login, string captchaCode)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            // Validate CAPTCHA
            var storedCaptchaCode = HttpContext.Session.GetString("CaptchaCode");
            if (string.IsNullOrEmpty(storedCaptchaCode) || !string.Equals(storedCaptchaCode, captchaCode, StringComparison.OrdinalIgnoreCase))
            {
                _notification.Error("Invalid CAPTCHA. Please try again.");
                return View(login);
            }

            // Check if the user exists
            var existingUser = await _userManager.FindByNameAsync(login.Username);
            if (existingUser == null)
            {
                _notification.Error("Username does not exist");
                return View(login);
            }
            if (!await _userManager.IsEmailConfirmedAsync(existingUser))
            {
                _notification.Error("Your email is not confirmed. Please check your email to confirm your account.");
                return View(login);
            }

            // Perform password sign-in
            var result = await _signInManager.PasswordSignInAsync(existingUser, login.Password ?? string.Empty, login.RememberMe, true);

            if (result.Succeeded)
            {
                var deviceIdentifier = HttpContext.Connection.RemoteIpAddress?.ToString();
                var device = _context.UserDevices.FirstOrDefault(d => d.UserId == existingUser.Id && d.DeviceIdentifier == deviceIdentifier);
                if (device == null || !device.IsVerified)
                {
                    var otp = GenerateOTP();
                    var otpExpiration = DateTime.UtcNow.AddMinutes(5);
                    HttpContext.Session.SetString("OTP", otp);
                    HttpContext.Session.SetString("OTPExpiration", otpExpiration.ToString());
                    HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
                    await _emailService.SendEmailAsync(existingUser.Email, "Login Verification Code",
                    $"Your OTP is: {otp}. It is valid for 5 minutes.");

                    if (device == null)
                    {
                        device = new UserDevice
                        {
                            UserId = existingUser.Id,
                            DeviceIdentifier = deviceIdentifier,
                            LastLoggedIn = DateTime.UtcNow,
                            IsVerified = false
                        };
                        _context.UserDevices.Add(device);
                        await _context.SaveChangesAsync();
                    }
                    // Redirect to OTP verification page
                    TempData["DeviceId"] = device.Id;
                    return View("Login", new LoginVM { RequiresOtpVerification = true });
                }
                await FinalizeAuthentication(existingUser, login.RememberMe);
                    _notification.Success("Login Successful");
                    return RedirectToAction("Index", "Home");
                
            }   
            else if (result.IsLockedOut)
            {
                _notification.Error("Your account is locked. Please try again later.");
                return View(login);
            }
            else
            {
                _notification.Error("Invalid login attempt.");
                return View(login);
            }
        }

        private async Task FinalizeAuthentication(ApplicationUser user, bool rememberMe) {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme); // Sign out current user
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            //create a handler if user tries to logout even when not authenticated
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                _notification.Error("You are not logged in.");
                return RedirectToAction("Index", "Home");
            }
            await _signInManager.SignOutAsync();
            _notification.Success("Logout Successful");
            //return redirect to current page being navigated
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Register")]
        public async Task<IActionResult> Register() {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View(new RegisterVM());
            }
            _notification.Error("You are currently logged in, please log out first.");
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            var user = new ApplicationUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (result.Succeeded)
            {
                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

                // Send the email (replace this with your email service)
                await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");

                _notification.Success("Registration successful! Please check your email to confirm your account.");
                return RedirectToAction("Login");   
            }

            foreach (var error in result.Errors)
            {
                Console.WriteLine(string.Empty, error.Description);
            }

            return View(registerVM);
        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid email confirmation request.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                _notification.Success("Email confirmed successfully! You can now log in.");
                return RedirectToAction("Login");
            }

            _notification.Error("Failed to confirm email. The link may have expired or is invalid.");
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(string otp)
        {
            var storedOtp = HttpContext.Session.GetString("OTP");
            var otpExpiration = DateTime.Parse(HttpContext.Session.GetString("OTPExpiration") ?? DateTime.MinValue.ToString());

            if (string.IsNullOrEmpty(storedOtp) || DateTime.UtcNow > otpExpiration)
            {
                _notification.Error("OTP expired. Please log in again.");
                return RedirectToAction("Login");
            }

            if (otp != storedOtp)
            {
                _notification.Error("Invalid OTP. Please try again.");
                return RedirectToAction();
            }

            // Mark the device as verified
            var deviceId = int.Parse(TempData["DeviceId"]?.ToString() ?? "0");
            var device = await _context.UserDevices.FindAsync(deviceId);
            if (device != null)
            {
                device.IsVerified = true;
                device.LastLoggedIn = DateTime.UtcNow;
                _context.Update(device);
                await _context.SaveChangesAsync();
            }
            // Finalize authentication
            var userId = HttpContext.Session.GetString("UserId");
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await FinalizeAuthentication(user, false); // Adjust rememberMe as needed
                _notification.Success("OTP Verified. Login successful.");
                return RedirectToAction("Index", "Home");
            }

            // Clear OTP from session
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("OTPExpiration");

            return RedirectToAction("Login");
        }

        private string GenerateOTP() {
            var data = new byte[6];
            RandomNumberGenerator.Fill(data);
            var otp = BitConverter.ToUInt32(data, 0) % 1_000_000;
            return otp.ToString("D6");
        }

        [HttpGet("GenerateCaptcha")]
        [AllowAnonymous]
        public IActionResult GenerateCaptcha()
        {
            var (captchaCode, captchaImage) = _captchaService.GenerateCaptcha();

            // Store the CAPTCHA code in TempData or Session
            TempData["CaptchaCode"] = captchaCode;

            return File(captchaImage, "image/png");
        }
    }
}
