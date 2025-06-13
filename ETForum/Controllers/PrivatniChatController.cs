using ETForum.Data;
using ETForum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using ETForum.Hubs;

namespace ETForum.Controllers
{
    public class PrivatniChatController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ETForumDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public PrivatniChatController(UserManager<Korisnik> userManager, ETForumDbContext context, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Chat(string prijateljId)
        {
            var mojId = _userManager.GetUserId(User);

            // Provjeri da li su prijatelji
            var prijateljstvo = await _context.Prijateljstva
                .AnyAsync(p => ((p.korisnik1Id == mojId && p.korisnik2Id == prijateljId) ||
                               (p.korisnik1Id == prijateljId && p.korisnik2Id == mojId)) &&
                               p.status == Status.PRIHVACENO);

            if (!prijateljstvo)
            {
                TempData["PorukaCrvena"] = "Možete chatovati samo sa prijateljima.";
                return RedirectToAction("ListaPrijatelja", "Prijateljstvo");
            }

            // Dohvati prijatelja
            var prijatelj = await _context.Users.FindAsync(prijateljId);
            if (prijatelj == null)
                return NotFound();

            // Dohvati poruke između korisnika
            var poruke = await _context.PrivatniChatovi
                .Include(p => p.posiljalac)
                .Include(p => p.primaoc)
                .Where(p => (p.posiljalacId == mojId && p.primaocId == prijateljId) ||
                           (p.posiljalacId == prijateljId && p.primaocId == mojId))
                .OrderBy(p => p.vrijeme)
                .ToListAsync();

            // Označi poruke kao pročitane
            var neprocitane = poruke.Where(p => p.primaocId == mojId && !p.procitano);
            foreach (var poruka in neprocitane)
            {
                poruka.procitano = true;
            }
            await _context.SaveChangesAsync();

            ViewBag.PrijateljId = prijateljId;
            ViewBag.PrijateljIme = prijatelj.UserName;
            ViewBag.MojId = mojId;

            return View(poruke);
        }

        [HttpPost]
        public async Task<IActionResult> PosaljiPoruku(string prijateljId, string poruka)
        {
            var mojId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(poruka))
                return RedirectToAction("Chat", new { prijateljId });

            // Provjeri prijateljstvo
            var prijateljstvo = await _context.Prijateljstva
                .AnyAsync(p => ((p.korisnik1Id == mojId && p.korisnik2Id == prijateljId) ||
                               (p.korisnik1Id == prijateljId && p.korisnik2Id == mojId)) &&
                               p.status == Status.PRIHVACENO);

            if (!prijateljstvo)
            {
                TempData["PorukaCrvena"] = "Možete slati poruke samo prijateljima.";
                return RedirectToAction("ListaPrijatelja", "Prijateljstvo");
            }

            var novaPoruka = new PrivatniChat
            {
                posiljalacId = mojId,
                primaocId = prijateljId,
                poruka = poruka,
                vrijeme = DateTime.Now,
                procitano = false
            };

            _context.PrivatniChatovi.Add(novaPoruka);
            await _context.SaveChangesAsync();

            // Pošalji poruku preko SignalR-a
            var posiljalac = await _context.Users.FindAsync(mojId);
            await _hubContext.Clients.User(prijateljId).SendAsync("ReceivePrivateMessage",
                posiljalac.UserName, poruka, DateTime.Now.ToString("HH:mm"));

            return RedirectToAction("Chat", new { prijateljId });
        }

        [HttpGet]
        public async Task<IActionResult> BrojNeprocitanih()
        {
            var mojId = _userManager.GetUserId(User);

            var broj = await _context.PrivatniChatovi
                .Where(p => p.primaocId == mojId && !p.procitano)
                .CountAsync();

            return Json(broj);
        }
    }
}