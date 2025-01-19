using System.Diagnostics;
using MichiBlog.Models;
using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MichiBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            const int pageSize = 4; // Default posts per page
            int pageNumber = page ?? 1;
            var vm = new HomeVM();
            var setting = _context.Settings!.ToList();
            vm.Title = setting[0].Title;
            vm.ShortDescription = setting[0].Description;
            vm.ThumbnailUrl = setting[0].ThumbnailUrl;
            vm.Posts = _context.Posts!
                        .Include(x => x.ApplicationUser)
                        .Include(x => x.Reactions)
                        .Include(x => x.Comments)
                        .OrderByDescending(x => x.CreatedDate)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            int totalPosts = _context.Posts!.Count();
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalPosts / pageSize);
            ViewData["CurrentPage"] = pageNumber;
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
