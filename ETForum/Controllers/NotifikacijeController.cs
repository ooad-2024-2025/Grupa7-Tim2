﻿namespace ETForum.Controllers
{
    using ETForum.Data;
    using ETForum.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc.Filters;

    [Authorize]
    public class NotifikacijeController : Controller
    {
        private readonly ETForumDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public NotifikacijeController(ETForumDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifikacije = await _context.Notifikacije
                .Where(n => n.KorisnikId == user.Id)
                .OrderByDescending(n => n.Vrijeme)
                .ToListAsync();

            return View(notifikacije);
        }

        [HttpPost]
        public async Task<IActionResult> OznačiKaoProcitano(int id)
        {
            var notifikacija = await _context.Notifikacije.FindAsync(id);
            if (notifikacija != null)
            {
                notifikacija.Procitano = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> IdiNaPitanje(int id)
        {
            var notifikacija = await _context.Notifikacije
                .Include(n => n.Korisnik)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notifikacija == null)
            {
                return NotFound(); 
            }

            return RedirectToAction("Details", "QnA", new { id = notifikacija.pitanjeId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> BrojNeprocitanih()
        {
            var userId = _userManager.GetUserId(User);
            var broj = await _context.Notifikacije
                .Where(n => n.KorisnikId == userId && !n.Procitano)
                .CountAsync();
            return Json(new { broj });
        }

    }

}
