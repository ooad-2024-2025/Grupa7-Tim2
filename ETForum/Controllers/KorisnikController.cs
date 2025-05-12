using ETForum.Data;
using Microsoft.AspNetCore.Mvc;
using ETForum.Models;
using ETForum.DTO;
using Microsoft.EntityFrameworkCore;

namespace ETForum.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly ETForumDbContext _context;
        public KorisnikController(ETForumDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Registracija()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registracija (RegistracijaDTO noviKorisnik)
        {
            if (!ModelState.IsValid) {
                return View();
            }

            bool nicknameZauzet = await _context.Korisnici.AnyAsync(k => k.nickname ==  noviKorisnik.nickname);
            if (nicknameZauzet)
            {
                ModelState.AddModelError("nickname", "Nickname je već zauzet!");
                return View();
            }

            bool emailZauzet = await _context.Korisnici.AnyAsync(k => k.email == noviKorisnik.email);
            if (emailZauzet)
            {
                ModelState.AddModelError("email", "E-mail je već zauzet!");
                return View();

            }

            var korisnik = new Korisnik
            {
                ime = noviKorisnik.ime,
                prezime = noviKorisnik.prezime,
                email = noviKorisnik.email,
                lozinka = noviKorisnik.lozinka,
                nickname = noviKorisnik.nickname,
                uloga = noviKorisnik.uloga,
                smjer = noviKorisnik.smjer
            };
            _context.Korisnici.Add(korisnik);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login () 
        {
            return View();
        }

        [HttpPost] 
        public async Task<IActionResult> Login (LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                bool ispravanLogin = await _context.Korisnici.AnyAsync(u => (u.nickname == loginDTO.nickname || u.email == loginDTO.email)
                && u.lozinka == loginDTO.lozinka);
                if (ispravanLogin)
                {
                    
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Neispravni podaci!");
            }
            return View();
        }
    }
}
