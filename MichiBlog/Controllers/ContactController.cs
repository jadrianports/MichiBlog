using MichiBlog.WebApp.Services;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class ContactController : Controller
{
    private readonly EmailService _emailService;

    public ContactController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(ContactMessageVM model)
    {
        if (!ModelState.IsValid)
        {
            return View("Contact", model);
        }

        string body = $@"You have received a new message on your blog:

            Name: {model.Name}
            Email: {model.Email}
            Phone: {model.Phone}

            Message:
            {model.Message}";

        try
        {
            await _emailService.SendEmailAsync(model.Email, "New Contact Message", body);
            TempData["SuccessMessage"] = "Your message has been sent successfully!";
            return RedirectToAction("Contact");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "There was an error sending your message: " + ex.Message;
            return RedirectToAction("Contact");
        }

        return RedirectToAction("Contact");
    }
}
