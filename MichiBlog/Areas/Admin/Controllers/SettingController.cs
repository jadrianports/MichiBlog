using AspNetCoreHero.ToastNotification.Abstractions;
using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.Models;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace MichiBlog.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingController(ApplicationDbContext context,
                                 INotyfService notification,
                                 IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notification = notification;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index() {
            var settings = _context.Settings!.ToList();
            if (settings.Count > 0) {
                var settingVM = new SettingVM()
                {
                    Id = settings[0].Id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    Description = settings[0].Description,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    FacebookUrl = settings[0].FacebookUrl,
                    TwitterUrl = settings[0].TwitterUrl,
                    InstagramUrl = settings[0].InstagramUrl

                };
                return View(settingVM);
            }
            var setting = new Setting()
            {
                SiteName = "Demo Name",
            };
            await _context.Settings!.AddAsync(setting);
            await _context.SaveChangesAsync();
            var createdSetting = _context.Settings!.ToList();
            var createdVM = new SettingVM()
            {
                Id = createdSetting[0].Id,
                SiteName = createdSetting[0].SiteName,
                Title = createdSetting[0].Title,
                Description = createdSetting[0].Description,
                ThumbnailUrl = createdSetting[0].ThumbnailUrl,
                FacebookUrl = createdSetting[0].FacebookUrl,
                TwitterUrl = createdSetting[0].TwitterUrl,
                InstagramUrl = createdSetting[0].InstagramUrl   
            };
            return View(createdVM);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SettingVM settingVM) {
            if (!ModelState.IsValid) {
                return View(settingVM);
            }
            var setting = await _context.Settings!.FirstOrDefaultAsync(x => x.Id == settingVM.Id + 1);
            if (setting == null) {
                _notification.Error("Something went wrong.");
                return View(settingVM);
            }
            setting.SiteName = settingVM.SiteName;
            setting.Title = settingVM.Title;
            setting.Description = settingVM.Description;
            setting.FacebookUrl = settingVM.FacebookUrl;
            setting.TwitterUrl = settingVM.TwitterUrl;
            setting.InstagramUrl = settingVM.InstagramUrl;

            if (settingVM.Thumbnail != null) {
                setting.ThumbnailUrl = UploadImage(settingVM.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Setting Updated Successfully");
            return RedirectToAction("Index", "Setting", new { area = "Admin" }); 
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
