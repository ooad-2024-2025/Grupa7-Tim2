using System.Diagnostics;
using ETForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Naslovna()
        {
            ViewBag.KorisnikIme = User.Identity?.Name ?? "Gost";
            return View();
        }

        public IActionResult QnA()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, Student")]
        public IActionResult StudyRoom()
        {
            return RedirectToAction("Index", "StudySessions");
        }

        public IActionResult LiveChat()
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
