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

    public class BanUserRequest
    {
        public string korisnikId { get; set; }
        public int brojDana { get; set; }
        public string razlog { get; set; }
    }


    public class KorisnikController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly ETForumDbContext _context;
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public KorisnikController(ETForumDbContext context, UserManager<Korisnik> userManager, SignInManager<Korisnik> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Naslovna", "Home"); // ili "Index", zavisno od tebe

            return View();
        }

        [HttpPost] 
        public async Task<IActionResult> Login (LoginDTO loginDTO)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Naslovna", "Home"); 

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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Uspješno ste se odjavili!";
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MojProfil(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = _userManager.GetUserId(User);

            ViewBag.BrojPitanja = await _context.Pitanja.CountAsync(p => p.korisnikId == id);
            ViewBag.BrojOdgovora = await _context.Odgovori.CountAsync(o => o.korisnikId == id);
            ViewBag.BrojKomentara = await _context.Komentari.CountAsync(k => k.korisnikId == id);
            ViewBag.BrojPrijatelja = await _context.Prijateljstva.CountAsync(p => (p.korisnik1Id == id || p.korisnik2Id == id) && p.status == Status.PRIHVACENO);
            ViewBag.MojaPitanja = await _context.Pitanja.Where(p => p.korisnikId == id).ToListAsync();


            var korisnik = await _context.Korisnici
                .Include(k => k.KorisnikDostignuca)
                .ThenInclude(kd => kd.Dostignuce)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (korisnik == null)
                return NotFound();

            var prijateljstva = await _context.Prijateljstva
                .Include(p => p.korisnik1)
                .Include(p => p.korisnik2)
                .Where(p =>
                    (p.korisnik1Id == id || p.korisnik2Id == id) &&
                    p.status == Status.PRIHVACENO)
                .ToListAsync();

            var prijatelji = prijateljstva.Select(p =>
                p.korisnik1Id == id ? p.korisnik2 : p.korisnik1
            ).ToList();

            ViewBag.Prijatelji = prijatelji;

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

            await _userManager.UpdateAsync(korisnik);

            TempData["SuccessMessage"] = "Profil uspješno ažuriran!";
            return RedirectToAction("MojProfil");
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

        [Authorize(Roles = "Administrator")]
        public IActionResult Kreiraj()
        {
            ViewBag.RoleList = new List<string> { "Student", "Asistent", "Profesor", "Administrator" };
            ViewBag.Smjerovi = Enum.GetValues(typeof(Smjer));   // OBAVEZNO
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Kreiraj(
            string ime,
            string prezime,
            string nickname,
            string email,
            string password,
            string role,
            Smjer? smjer,
            string? urlSlike
        )
        {
            ViewBag.RoleList = new List<string> { "Student", "Asistent", "Profesor", "Administrator" };
            ViewBag.Smjerovi = Enum.GetValues(typeof(Smjer));

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("", "Email, šifra i rola su obavezni!");
                return View();
            }

            var postoji = await _userManager.FindByEmailAsync(email);
            if (postoji != null)
            {
                ModelState.AddModelError("", "Korisnik sa ovim emailom već postoji!");
                return View();
            }

            var novi = new Korisnik
            {
                UserName = email,
                Email = email,
                ime = ime,
                prezime = prezime,
                nickname = nickname,
                smjer = smjer,
                datumRegistracije = DateTime.Now,
                urlSlike = urlSlike,
                podesenProfil = false,
                lastLogin = null
            };

            var result = await _userManager.CreateAsync(novi, password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(novi, role);
                TempData["PorukaZelena"] = "Korisnik uspješno kreiran!";
                return RedirectToAction("Naslovna", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View();
            }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SviKorisnici()
        {
            var korisnici = await _userManager.Users.ToListAsync();
            return View(korisnici);
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Obrisi(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["PorukaCrvena"] = "Korisnik ne postoji!";
                return RedirectToAction("SviKorisnici");
            }

            if (user.UserName == User.Identity.Name)
            {
                TempData["PorukaCrvena"] = "Ne možete obrisati sami sebe!";
                return RedirectToAction("SviKorisnici");
            }

            // 1. Prvo obriši sve povezane entitete
            var pitanja = _context.Pitanja.Where(p => p.korisnikId == user.Id);
            _context.Pitanja.RemoveRange(pitanja);

            var odgovori = _context.Odgovori.Where(o => o.korisnikId == user.Id);
            _context.Odgovori.RemoveRange(odgovori);

            var komentari = _context.Komentari.Where(k => k.korisnikId == user.Id);
            _context.Komentari.RemoveRange(komentari);

            var ocjene = _context.OcjenaPredmeta.Where(o => o.korisnikId == user.Id);
            _context.OcjenaPredmeta.RemoveRange(ocjene);

            var prijateljstva = _context.Prijateljstva.Where(p => p.korisnik1Id == user.Id || p.korisnik2Id == user.Id);
            _context.Prijateljstva.RemoveRange(prijateljstva);

            var korisnikDostignuca = _context.KorisnikDostignuca.Where(kd => kd.korisnikId == user.Id);
            _context.KorisnikDostignuca.RemoveRange(korisnikDostignuca);

            var privatniChatovi = _context.PrivatniChatovi.Where(kd => kd.posiljalacId == user.Id);
            _context.PrivatniChatovi.RemoveRange(privatniChatovi);

            privatniChatovi = _context.PrivatniChatovi.Where(kd => kd.primaocId == user.Id);
            _context.PrivatniChatovi.RemoveRange(privatniChatovi);

            var studySession = _context.StudySession.Where(kd => kd.korisnikId == user.Id);
            _context.StudySession.RemoveRange(studySession);

            var liveChat = _context.LiveChat.Where(kd => kd.korisnikId == user.Id);
            _context.LiveChat.RemoveRange(liveChat);

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                TempData["PorukaZelena"] = "Korisnik uspješno obrisan!";
            else
                TempData["PorukaCrvena"] = "Greška prilikom brisanja korisnika!";

            return RedirectToAction("SviKorisnici");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> BanUser([FromForm] BanUserRequest model)
        {
            // Model: korisnikId, brojDana, razlog
            var user = await _userManager.FindByIdAsync(model.korisnikId);
            if (user == null)
                return NotFound();

            user.BanDo = DateTime.Now.AddDays(model.brojDana);
            user.BanRazlog = model.razlog;
            await _userManager.UpdateAsync(user);

            return Ok();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PromijeniSliku(IFormFile slika)
        {
            if (slika == null || slika.Length == 0)
            {
                TempData["PorukaCrvena"] = "Morate odabrati sliku.";
                return RedirectToAction("MojProfil");
            }

            // Samo slike dopuštene!
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(slika.FileName).ToLower();
            if (!allowed.Contains(ext))
            {
                TempData["PorukaCrvena"] = "Dozvoljeni su samo jpg, png, gif fajlovi!";
                return RedirectToAction("MojProfil");
            }

            // Jedinstveno ime
            var fileName = $"{Guid.NewGuid()}{ext}";
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var path = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await slika.CopyToAsync(stream);
            }

            // Sačuvaj url u bazu
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["PorukaCrvena"] = "Nije pronađen korisnik!";
                return RedirectToAction("MojProfil");
            }
            user.urlSlike = "/uploads/profile/" + fileName;
            await _userManager.UpdateAsync(user);

            TempData["PorukaZelena"] = "Profilna slika uspješno promijenjena!";
            return RedirectToAction("MojProfil");
        }

    }
}
