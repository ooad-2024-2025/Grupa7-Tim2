﻿using ETForum.Data;
using ETForum.Helper;
using ETForum.Hubs;
using ETForum.Models;
using ETForum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace ETForum.Controllers
{
    public class PrijateljstvoController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ETForumDbContext _context;
        private readonly DostignucaHelper _dostignucaHelper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly EmailSender _emailSender;

        public PrijateljstvoController(UserManager<Korisnik> userManager, ETForumDbContext context, DostignucaHelper dostignucaHelper, IHubContext<NotificationHub> hubContext, EmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _dostignucaHelper = dostignucaHelper;
            _hubContext = hubContext;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DodajPrijatelja(string Id, string unos1)
        {
            var mojId = _userManager.GetUserId(User);

            if (mojId == Id)
            {
                TempData["PorukaCrvena"] = "Ne možete sami sebi poslati zahtjev za prijateljstvo.";
                return RedirectToAction("PretragaKorisnika", "Korisnik", new { unos = unos1 });
            }

            var postoji = await _context.Prijateljstva.AnyAsync(p =>
                (p.korisnik1Id == mojId && p.korisnik2Id == Id) ||
                (p.korisnik1Id == Id && p.korisnik2Id == mojId));

            if (postoji)
            {
                TempData["PorukaCrvena"] = "Zahtjev je već poslan ili ste već prijatelji.";
                return RedirectToAction("PretragaKorisnika", "Korisnik", new { unos = unos1 });
            }

            var zahtjev = new Prijateljstvo
            {
                korisnik1Id = mojId,
                korisnik2Id = Id,
                status = Status.ZATRAZENO
            };

            _context.Prijateljstva.Add(zahtjev);
            await _context.SaveChangesAsync();

            var notifikacija = new Notifikacija
            {
                KorisnikId = Id,
                Tekst = "Dobili ste novi zahtjev za prijateljstvo.",
                Link = Url.Action("PrimljeniZahtjevi", "Prijateljstvo"),
                Procitano = false,
                Vrijeme = DateTime.Now
            };
            _context.Notifikacije.Add(notifikacija);
            await _context.SaveChangesAsync();

            // EMAIL
            var primalac = await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
            var posiljalac = await _userManager.GetUserAsync(User);

            if (primalac != null && posiljalac != null)
            {
                await _emailSender.PosaljiEmailAsync(primalac.Email,
                    "Novi zahtjev za prijateljstvo",
                    $"Pozdrav {primalac.nickname},<br><br>" +
                    $"Korisnik <b>{posiljalac.nickname}</b> vam je poslao zahtjev za prijateljstvo.<br>" +
                    $"<a href='{Url.Action("PrimljeniZahtjevi", "Prijateljstvo", null, Request.Scheme)}'>Pogledaj zahtjev</a>");
            }

            await _hubContext.Clients.User(Id).SendAsync("ReceiveNotification", "Dobili ste novi zahtjev za prijateljstvo.");

            TempData["Poruka"] = "Zahtjev za prijateljstvo je poslan.";
            return RedirectToAction("PretragaKorisnika", "Korisnik", new { unos = unos1 });
        }


        [HttpGet]
        public async Task<IActionResult> PrimljeniZahtjevi()
        {
            var mojId = _userManager.GetUserId(User);
            var zahtjevi = await _context.Prijateljstva
                .Include(p => p.korisnik1)
                .Where(p => p.korisnik2Id == mojId && p.status == Status.ZATRAZENO)
                .ToListAsync();

            return View(zahtjevi);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PrihvatiZahtjev(int id)
        {
            var zahtjev = await _context.Prijateljstva.FindAsync(id);
            if (zahtjev == null)
                return NotFound();

            // Postavljanje statusa zahtjeva na prihvaćeno
            zahtjev.status = Status.PRIHVACENO;
            await _context.SaveChangesAsync();

            // Provjeri je li korisnik postigao prvo prijateljstvo za oba korisnika
            var mojId = _userManager.GetUserId(User);


            await _dostignucaHelper.ProvjeriDostignucaZaKorisnika(mojId);
            await _dostignucaHelper.ProvjeriDostignucaZaKorisnika(zahtjev.korisnik1Id);

            var notifikacija = new Notifikacija
            {
                KorisnikId = zahtjev.korisnik1Id, 
                Tekst = "Vaš zahtjev za prijateljstvo je prihvaćen.",
                Link = Url.Action("ListaPrijatelja", "Prijateljstvo"),
                Procitano = false,
                Vrijeme = DateTime.Now
            };
            _context.Notifikacije.Add(notifikacija);

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();


            await _hubContext.Clients.User(zahtjev.korisnik1Id).SendAsync("ReceiveNotification", "Vaš zahtjev za prijateljstvo je prihvaćen!");

            TempData["PorukaZelena"] = "Zahtjev je prihvaćen.";
            return RedirectToAction("PrimljeniZahtjevi");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> OdbijZahtjev(int id)
        {
            var zahtjev = await _context.Prijateljstva.FindAsync(id);
            if (zahtjev == null)
                return NotFound();

            _context.Prijateljstva.Remove(zahtjev);
            await _context.SaveChangesAsync();

            TempData["PorukaCrvena"] = "Zahtjev je odbijen.";
            return RedirectToAction("PrimljeniZahtjevi");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListaPrijatelja()
        {
            var mojId = _userManager.GetUserId(User);

            var prijateljstva = await _context.Prijateljstva
                .Include(p => p.korisnik1)
                .Include(p => p.korisnik2)
                .Where(p => (p.korisnik1Id == mojId || p.korisnik2Id == mojId) && p.status == Status.PRIHVACENO)
                .ToListAsync();

            ViewBag.MojId = mojId; 

            return View(prijateljstva);
        }


    }
}
