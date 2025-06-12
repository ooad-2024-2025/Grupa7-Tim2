using ETForum.Data;
using ETForum.DTO;
using ETForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ETForum.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ETForumDbContext _context;
        private readonly SignInManager<Korisnik> _signInManager;
        public KorisnikController(ETForumDbContext context, UserManager<Korisnik> userManager, SignInManager<Korisnik> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Registracija()
        {
            return View(new RegistracijaDTO());
        }
        [HttpPost]
        public async Task<IActionResult> Registracija (RegistracijaDTO noviKorisnik)
        {
            if (!ModelState.IsValid) {
                return View(noviKorisnik);
            }

            bool nicknameZauzet = await _context.Korisnici.AnyAsync(k => k.nickname ==  noviKorisnik.nickname);
            if (nicknameZauzet)
            {
                ModelState.AddModelError("nickname", "Nickname je već zauzet!");
                return View(noviKorisnik);
            }

            bool emailZauzet = await _context.Korisnici.AnyAsync(k => k.Email == noviKorisnik.email);
            if (emailZauzet)
            {
                ModelState.AddModelError("email", "E-mail je već zauzet!");
                return View(noviKorisnik);
            }

            var korisnik = new Models.Korisnik
            {
                ime = noviKorisnik.ime,
                prezime = noviKorisnik.prezime,
                Email = noviKorisnik.email,
                nickname = noviKorisnik.nickname,
                UserName = noviKorisnik.nickname,
                uloga = noviKorisnik.uloga,
                smjer = noviKorisnik.smjer,
                datumRegistracije = DateTime.Now,
                podesenProfil = false
            };
            var result = await _userManager.CreateAsync(korisnik, noviKorisnik.lozinka);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(noviKorisnik);
            }
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
                var korisnik = await _userManager.FindByNameAsync(loginDTO.nickname)
               ?? await _userManager.FindByEmailAsync(loginDTO.email);

                if (korisnik != null)
                {
                    var passwordValid = await _userManager.CheckPasswordAsync(korisnik, loginDTO.lozinka);

                    if (passwordValid)
                    {

                        korisnik.lastLogin = DateTime.Now;
                        _context.Update(korisnik);
                        await _context.SaveChangesAsync();

                        await _signInManager.SignInAsync(korisnik, isPersistent: false);
                        TempData["SuccessMessage"] = "Uspješno ste se prijavili!";
                        if (!korisnik.podesenProfil)
                        {
                            return RedirectToAction("PodesiProfil", "Korisnik");
                        }
                        return RedirectToAction("Naslovna", "Home");
                    }
                }
                TempData["ErrorMessage"] = "Prijava nije uspjela!";
                ModelState.AddModelError("", "Neispravni podaci!");
            }
            return View();
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PodesiProfil()
        {
            var userId = _userManager.GetUserId(User);

            var korisnik = await _userManager.FindByIdAsync(userId);

            if (korisnik != null && korisnik.podesenProfil)
                return RedirectToAction("Naslovna", "Home");
            if (korisnik != null)
            {
                ViewBag.KorisnikIme = User.Identity?.Name ?? "Gost";
                return View(korisnik);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PodesiProfil (Models.Korisnik korisnik, IFormFile profilnaSlika)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != korisnik.Id) return Unauthorized();

            var korisnikIzBaze = _context.Korisnici.FirstOrDefault(k => k.Id == korisnik.Id);
            if (korisnikIzBaze == null) return NotFound();
            
            if(profilnaSlika != null && profilnaSlika.Length > 0)
            {
                var fileName = Path.GetFileName(profilnaSlika.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    profilnaSlika.CopyTo(stream);
                }
                korisnikIzBaze.urlSlike = "/images/" + fileName;
            }

            korisnikIzBaze.ime = korisnik.ime;
            korisnikIzBaze.prezime = korisnik.prezime;
            korisnikIzBaze.nickname = korisnik.nickname;
            korisnikIzBaze.UserName = korisnik.nickname;
            korisnikIzBaze.Email = korisnik.Email;
            korisnikIzBaze.smjer = korisnik.smjer;
            korisnikIzBaze.podesenProfil = true;

            await _userManager.UpdateAsync(korisnikIzBaze);

            return RedirectToAction("Naslovna", "Home"); 
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Uspješno ste se odjavili!";
            return RedirectToAction("Login");
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MojProfil()
        {
            var userId = _userManager.GetUserId(User);

            var korisnik = await _context.Korisnici
                .Include(k => k.KorisnikDostignuca).
                ThenInclude(kd => kd.Dostignuce)
                .FirstOrDefaultAsync(k => k.Id == userId);

            if (korisnik == null)
                return NotFound();

            return View(korisnik);
        }

        //GET
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UrediProfil()
        {
            var userId = _userManager.GetUserId(User);
            var korisnik = await _context.Korisnici.FirstOrDefaultAsync(k => k.Id == userId);

            if (korisnik == null) return NotFound();

            return View(korisnik);
        }

        //POST
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UrediProfil(Korisnik izmjene, IFormFile? novaSlika)
        {
            var userId = _userManager.GetUserId(User);
            var korisnik = await _context.Korisnici.FirstOrDefaultAsync(k => k.Id == userId);

            if (korisnik == null) return NotFound();

            korisnik.ime = izmjene.ime;
            korisnik.prezime = izmjene.prezime;
            korisnik.nickname = izmjene.nickname;
            korisnik.UserName = izmjene.nickname;
            korisnik.Email = izmjene.Email;
            korisnik.smjer = izmjene.smjer;

            if (novaSlika != null && novaSlika.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(novaSlika.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await novaSlika.CopyToAsync(stream);
                }

                korisnik.urlSlike = "/images/" + fileName;
            }

        [HttpGet]
        public async Task<IActionResult> PretragaKorisnika(string unos)
        {
            ViewBag.SearchTerm = unos;

            var korisnici = await _context.Korisnici
                            .Where(k => k.UserName.Contains(unos))
                            .ToListAsync();
            return View(korisnici);
        }

            await _userManager.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Profil uspješno ažuriran!";
            return RedirectToAction("MojProfil");
        }

    }
}
