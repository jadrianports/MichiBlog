using Ganss.Xss;

namespace MichiBlog.WebApp.Services
{
    public class HtmlSanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();
        }
    }
}
