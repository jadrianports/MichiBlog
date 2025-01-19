using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using MichiBlog.WebApp.Services;
using MichiBlog.WebApp.Interfaces;

namespace MichiBlog.WebApp.Controllers
{
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public PageController(ApplicationDbContext context,
                              IEmailService emailService) {
            _context = context;
            _emailService = emailService;
        }
        public async Task<IActionResult> About()
        {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about");
            var pageVM = new PageVM()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Content = page.Content,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(pageVM);
        }

        public async Task<IActionResult> Contact()
        {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact");
            var pageVM = new PageVM()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Content = page.Content,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(pageVM);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(ContactMessageVM model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Validation failed: " + string.Join(", ", errors);
                return RedirectToAction("Contact");
            }

            string body = $@"You have received a new message on your blog:

            Name: {model.Name}
            Email: {model.Email}
            Phone: {model.Phone}

            Message:
            {model.Message}";

            try
            {
                await _emailService.SendEmailAsync("jadrianporter@gmail.com", "New Contact Message", body);
                TempData["SuccessMessage"] = "Your message has been sent successfully!";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There was an error sending your message: " + ex.Message;
                return RedirectToAction("Contact");
            }
        }

        public async Task<IActionResult> Privacy() 
        {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "privacy");
            var pageVM = new PageVM()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Content = page.Content,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(pageVM);
        }
    }
}
