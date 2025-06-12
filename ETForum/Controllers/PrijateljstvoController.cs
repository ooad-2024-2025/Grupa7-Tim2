using ETForum.Data;
using ETForum.Helper;
using ETForum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETForum.Controllers
{
    public class PrijateljstvoController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ETForumDbContext _context;
        private readonly DostignucaHelper _dostignucaHelper;

        public PrijateljstvoController(UserManager<Korisnik> userManager, ETForumDbContext context, DostignucaHelper dostignucaHelper)
        {
            _userManager = userManager;
            _context = context;
            _dostignucaHelper = dostignucaHelper;
        }

        [HttpPost]
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

            // Prvo ćemo provjeriti broj prijatelja korisnika koji prihvata zahtjev (mojId)
            var brojPrijateljaMojId = await _context.Prijateljstva
                .Where(p => (p.korisnik1Id == mojId || p.korisnik2Id == mojId) && p.status == Status.PRIHVACENO)
                .CountAsync();

            // Također ćemo provjeriti broj prijatelja korisnika koji je poslao zahtjev (korisnik2Id)
            var brojPrijateljaKorisnik2Id = await _context.Prijateljstva
                .Where(p => (p.korisnik1Id == zahtjev.korisnik2Id || p.korisnik2Id == zahtjev.korisnik2Id) && p.status == Status.PRIHVACENO)
                .CountAsync();

            // Ako je to prvo prijateljstvo za bilo kojeg korisnika, dodajemo dostignuće
            if (brojPrijateljaMojId == 1)
            {
                // Ako je mojId stekao svog prvog prijatelja, dodaj dostignuće
                await _dostignucaHelper.ProvjeriDostignucaZaKorisnika(mojId);
            }

            if (brojPrijateljaKorisnik2Id == 1)
            {
                // Ako je korisnik2Id stekao svog prvog prijatelja, dodaj dostignuće
                await _dostignucaHelper.ProvjeriDostignucaZaKorisnika(zahtjev.korisnik2Id);
            }

            TempData["PorukaZelena"] = "Zahtjev je prihvaćen.";
            return RedirectToAction("PrimljeniZahtjevi");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
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
