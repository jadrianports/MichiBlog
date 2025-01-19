using MichiBlog.Models;
using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MichiBlog.WebApp.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task Initialize()
        {
            if (!await _roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin))
            {
                // Create roles
                await _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin));
                await _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteUser));

                // Create admin user
                var user = new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin",
                };
                var result = await _userManager.CreateAsync(user, "Admin@0011");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, WebsiteRoles.WebsiteAdmin);
                }

                // Add default pages
                var listOfPages = new List<Page>()
        {
            new Page()
            {
                Title = "About Us",
                Slug = "about"
            },
            new Page()
            {
                Title = "Contact Us",
                Slug = "contact"
            },
            new Page()
            {
                Title = "Privacy Policy",
                Slug = "privacy"
            }
        };
                await _context.Pages.AddRangeAsync(listOfPages);

                // Add default settings if none exist
                if (!await _context.Settings.AnyAsync())
                {
                    var settings = new List<Setting>
            {
                new Setting
                {
                    SiteName = "My Website",
                    Title = "Welcome to my website",
                    Description = "This is a brief description of my website"
                }
            };
                    await _context.Settings.AddRangeAsync(settings);
                }

                // Save changes for pages and settings
                await _context.SaveChangesAsync();
            }
        }


    }
}
