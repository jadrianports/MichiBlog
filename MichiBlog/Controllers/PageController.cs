using Microsoft.AspNetCore.Mvc;

namespace MichiBlog.WebApp.Controllers
{
    public class PageController : Controller
    {
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy() 
        {
            return View();
        }
    }
}
