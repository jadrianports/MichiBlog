using AspNetCoreHero.ToastNotification.Abstractions;
using Ganss.Xss;
using MichiBlog.Models;
using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MichiBlog.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PageController : Controller
    {
        private readonly HtmlSanitizer _sanitizer;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public INotyfService _notification { get; }

        public PageController(ApplicationDbContext context,
                            HtmlSanitizer sanitizer,
                            INotyfService notification,
                            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _sanitizer = sanitizer;
            _notification = notification;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> About()
        {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about");
            var aboutVM = new PageVM() {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Content = page.Content,
                ThumbnailUrl = page.ThumbnailUrl

            };
            return View(aboutVM);
        }

        [HttpPost]
        public async Task<IActionResult> About(PageVM aboutVM)
        {
            if (!ModelState.IsValid)
            {
                return View(aboutVM);
            }
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "about");
            if (page == null)
            { 
                _notification.Error("About Page Not Found");
                return View();
            }

            page!.Title = aboutVM.Title;
            page.ShortDescription = _sanitizer.Sanitize(aboutVM.ShortDescription);
            page.Content = _sanitizer.Sanitize(aboutVM.Content);

            if (aboutVM.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(aboutVM.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("About Page Updated Successfully");
            return RedirectToAction("About", "Page", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Contact() {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact");
            var contactVM = new PageVM()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Content = page.Content,
                ThumbnailUrl = page.ThumbnailUrl

            };
            return View(contactVM);
        }
        [HttpPost]
        public async Task<IActionResult> Contact(PageVM contactVM) {
            if (!ModelState.IsValid)
            {
                return View(contactVM);
            }
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "contact");
            if (page == null)
            {
                _notification.Error("Contact Page Not Found");
                return View();
            }
            page!.Title = contactVM.Title;
            page.ShortDescription = _sanitizer.Sanitize(contactVM.ShortDescription);
            page.Content = _sanitizer.Sanitize(contactVM.Content);

            if (contactVM.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(contactVM.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Contact Page Updated Successfully");
            return RedirectToAction("Contact", "Page", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Privacy() {
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "privacy");
            var privacyVM = new PageVM() {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Content = page.Content,
                ThumbnailUrl = page.ThumbnailUrl
            };
            return View(privacyVM);
        }
        [HttpPost]
        public async Task<IActionResult> Privacy(PageVM privacyVM) {
            if (!ModelState.IsValid)
            {
                return View(privacyVM);
            }
            var page = await _context.Pages!.FirstOrDefaultAsync(x => x.Slug == "privacy");
            if (page == null)
            {
                _notification.Error("Privacy Page Not Found");
                return View();
            }
            page!.Title = privacyVM.Title;
            page.ShortDescription = _sanitizer.Sanitize(privacyVM.ShortDescription);
            page.Content = _sanitizer.Sanitize(privacyVM.Content);

            if (privacyVM.Thumbnail != null)
            {
                page.ThumbnailUrl = UploadImage(privacyVM.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Privacy Page Updated Successfully");
            return RedirectToAction("Privacy", "Page", new { area = "Admin" });
        }

        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = null;
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
