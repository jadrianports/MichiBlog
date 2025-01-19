using MichiBlog.WebApp.Interfaces;
using MichiBlog.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace MichiBlog.WebApp.Controllers
{
    [ApiController]
    [Route("api/captcha")]
    public class CaptchaController : Controller
    {
        private readonly ICaptchaService _captchaService;

        public CaptchaController(ICaptchaService captchaService) {
            _captchaService = captchaService;
        }
        [HttpGet]
        public IActionResult GetCaptcha()
        {
            try
            {
                var (code, image) = _captchaService.GenerateCaptcha();

                // Store CAPTCHA code in session
                HttpContext.Session.SetString("CaptchaCode", code);

                return File(image, "image/png");
            }
            catch (Exception ex) { 
                Console.WriteLine($"Error generating CAPTCHA: {ex.Message}");
                throw;
            }
        }
    }
}
