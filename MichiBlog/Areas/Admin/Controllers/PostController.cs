using AspNetCoreHero.ToastNotification.Abstractions;
using Ganss.Xss;
using MichiBlog.Models;
using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.Models;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MichiBlog.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly HtmlSanitizer _sanitizer;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;  
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notification { get; }

        public PostController(ApplicationDbContext context,
                            HtmlSanitizer sanitizer,
                            INotyfService notification,
                            IWebHostEnvironment webHostEnvironment,
                            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _sanitizer = sanitizer;
            _notification = notification;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = new List<Post>();

            posts = await _context.Posts!.Include(x => x.ApplicationUser).ToListAsync();
            var postVMs = posts.Select(x => new PostVM()
            {
                Id = x.Id,
                Title = x.Title!,
                ShortDescription = x.ShortDescription!,
                Author = x.ApplicationUser!.FirstName + " " + x.ApplicationUser.LastName,
                CreatedDate = x.CreatedDate,
                ThumbnailUrl = x.ThumbnailUrl
            }).ToList();

            return View(postVMs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM createPostVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createPostVM);
            }
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);


            var post = new Post();

            post.Title = createPostVM.Title;
            post.ShortDescription = _sanitizer.Sanitize(createPostVM.ShortDescription);
            post.Description = _sanitizer.Sanitize(createPostVM.Description);
            post.ApplicationUserId = loggedInUser!.Id;

            if (!string.IsNullOrWhiteSpace(post.Title))
            {
                string slug = post.Title.Trim().Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }

            if (createPostVM.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(createPostVM.Thumbnail);
            }

            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post created successfully");
            return RedirectToAction("Index", "Post");
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

        //[HttpPost]
        //public async Task<IActionResult> Create(CreatePostVM createPostVM) {
        //    if(!ModelState.IsValid) {
                
                
        //}
    }
}
