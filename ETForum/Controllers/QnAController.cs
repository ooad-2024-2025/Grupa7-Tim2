using ETForum.Data;
using ETForum.Helper;
using ETForum.Models;
using ETForum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ETForum.Controllers
{
    [Authorize]
    public class QnAController : Controller
    {
        private readonly ETForumDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DostignucaHelper _dostignucaHelper;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png", ".gif" };
        private readonly EmailSender _emailSender;

        public QnAController(ETForumDbContext context, UserManager<Korisnik> userManager, IWebHostEnvironment webHostEnvironment, DostignucaHelper dostignucaHelper, EmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _dostignucaHelper = dostignucaHelper;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string searchTerm, int? predmetId)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["PopularitySortParm"] = sortOrder == "popularity" ? "popularity_desc" : "popularity";

            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentPredmet"] = predmetId;

            var predmeti = await _context.Predmeti.ToListAsync();
            ViewBag.Predmeti = new SelectList(predmeti, "id", "naziv", predmetId);

            var questions = _context.Pitanja.Include(q => q.autor).Include(q => q.predmet).Include(q => q.Odgovori).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                questions = questions.Where(q => q.tekst.Contains(searchTerm) || (q.naslov != null && q.naslov.Contains(searchTerm)));
            }

            if (predmetId.HasValue)
            {
                questions = questions.Where(q => q.predmetId == predmetId.Value);
            }

            switch (sortOrder)
            {
                case "date_desc": questions = questions.OrderBy(q => q.datumPitanja); break;
                case "popularity": questions = questions.OrderBy(q => q.brojLajkova); break;
                case "popularity_desc": questions = questions.OrderByDescending(q => q.brojLajkova); break;
                default: questions = questions.OrderByDescending(q => q.datumPitanja); break;
            }

            return View(await questions.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Predmeti = new SelectList(await _context.Predmeti.ToListAsync(), "id", "naziv");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("naslov,tekst,predmetId")] Pitanje pitanje, IFormFile? file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null) return Challenge();

                    pitanje.korisnikId = user.Id;
                    pitanje.datumPitanja = DateTime.Now;
                    pitanje.brojLajkova = 0;

                    if (file != null && file.Length > 0)
                    {
                        var result = await HandleFileUpload(file);
                        if (result.Success)
                        {
                            pitanje.FilePath = result.FilePath;
                            pitanje.OriginalFileName = result.OriginalFileName;
                        }
                        else
                        {
                            ModelState.AddModelError("", result.ErrorMessage);
                            ViewBag.Predmeti = new SelectList(await _context.Predmeti.ToListAsync(), "id", "naziv", pitanje.predmetId);
                            return View(pitanje);
                        }
                    }

                    _context.Add(pitanje);
                    await _context.SaveChangesAsync();

                    await _dostignucaHelper.ProvjeriDostignucaZaKorisnika(user.Id);
                    TempData["SuccessMessage"] = "Pitanje je uspješno postavljeno.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch { ModelState.AddModelError("", "Došlo je do greške prilikom postavljanja pitanja. Pokušajte ponovo."); }

            ViewBag.Predmeti = new SelectList(await _context.Predmeti.ToListAsync(), "id", "naziv", pitanje.predmetId);
            return View(pitanje);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pitanje = await _context.Pitanja.Include(p => p.autor).Include(p => p.predmet).Include(p => p.Odgovori).ThenInclude(o => o.korisnik).FirstOrDefaultAsync(m => m.id == id);
            if (pitanje == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    ViewBag.HasLikedQuestion = await _context.PitanjeLajkovi.AnyAsync(pl => pl.pitanjeId == id && pl.korisnikId == user.Id);
                    ViewBag.LikedAnswers = await _context.OdgovorLajkovi.Where(ol => ol.korisnikId == user.Id).Select(ol => ol.odgovorId).ToListAsync();
                }
            }

            return View(pitanje);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnswer(int pitanjeId, [Bind("tekst")] Odgovor odgovor, IFormFile? file)
        {
            try
            {
                var pitanje = await _context.Pitanja.FindAsync(pitanjeId);
                if (pitanje == null) return NotFound();

                if (string.IsNullOrWhiteSpace(odgovor.tekst))
                {
                    TempData["ErrorMessage"] = "Tekst odgovora je obavezan.";
                    return RedirectToAction(nameof(Details), new { id = pitanjeId });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                odgovor.korisnikId = user.Id;
                odgovor.datumOdgovora = DateTime.Now;
                odgovor.brojLajkova = 0;
                odgovor.pitanjeId = pitanjeId;

                if (file != null && file.Length > 0)
                {
                    var result = await HandleFileUpload(file);
                    if (result.Success)
                    {
                        odgovor.FilePath = result.FilePath;
                        odgovor.OriginalFileName = result.OriginalFileName;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result.ErrorMessage;
                        return RedirectToAction(nameof(Details), new { id = pitanjeId });
                    }
                }

                _context.Odgovori.Add(odgovor);
                await _context.SaveChangesAsync();

                var autorPitanja = await _context.Users.FirstOrDefaultAsync(u => u.Id == pitanje.korisnikId);
                if (autorPitanja != null && autorPitanja.Id != user.Id)
                {
                    var notifikacija = new Notifikacija
                    {
                        KorisnikId = autorPitanja.Id,
                        Tekst = $"Korisnik {user.nickname} je odgovorio na tvoje pitanje \"{pitanje.naslov}\"",
                        Link = Url.Action("Details", "QnA", new { id = pitanje.id }, Request.Scheme),
                        Vrijeme = DateTime.Now,
                        Procitano = false,
                        pitanjeId = pitanje.id
                    };

                    _context.Notifikacije.Add(notifikacija);
                    await _context.SaveChangesAsync();

                    await _emailSender.PosaljiEmailAsync(autorPitanja.Email,
                        $"Novi odgovor na tvoje pitanje: {pitanje.naslov}",
                        $"Pozdrav {autorPitanja.nickname},<br><br>Korisnik <b>{user.nickname}</b> je odgovorio na tvoje pitanje.<br><a href='{notifikacija.Link}'>Pogledaj odgovor</a>");
                }

                await _dostignucaHelper.ProvjeriDostignucaZaKorisnika(user.Id);
                TempData["SuccessMessage"] = "Odgovor je uspješno postavljen.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Došlo je do greške prilikom postavljanja odgovora. Pokušajte ponovo.";
            }

            return RedirectToAction(nameof(Details), new { id = pitanjeId });
        }

        private async Task<FileUploadResult> HandleFileUpload(IFormFile file)
        {
            try
            {
                if (file.Length > _maxFileSize)
                    return new FileUploadResult { Success = false, ErrorMessage = "Veličina fajla prelazi dozvoljenih 5MB." };

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    return new FileUploadResult { Success = false, ErrorMessage = "Ovaj tip fajla nije dozvoljen." };

                var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new FileUploadResult { Success = true, FilePath = "/uploads/" + uniqueFileName, OriginalFileName = file.FileName };
            }
            catch
            {
                return new FileUploadResult { Success = false, ErrorMessage = "Došlo je do greške prilikom slanja fajla." };
            }
        }

        private class FileUploadResult
        {
            public bool Success { get; set; }
            public string? FilePath { get; set; }
            public string? OriginalFileName { get; set; }
            public string? ErrorMessage { get; set; }
        }
    }
}
