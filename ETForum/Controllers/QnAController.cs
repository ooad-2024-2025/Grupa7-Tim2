using Microsoft.AspNetCore.Mvc;
using ETForum.Data;
using ETForum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETForum.Controllers
{
    [Authorize]
    public class QnAController : Controller
    {
        private readonly ETForumDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png", ".gif" };

        public QnAController(ETForumDbContext context, UserManager<Korisnik> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

            var questions = _context.Pitanja
                .Include(q => q.autor)
                .Include(q => q.predmet)
                .Include(q => q.Odgovori)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                questions = questions.Where(q => q.tekst.Contains(searchTerm) || 
                                               (q.naslov != null && q.naslov.Contains(searchTerm)));
            }

            
            if (predmetId.HasValue)
            {
                questions = questions.Where(q => q.predmetId == predmetId.Value);
            }

            
            switch (sortOrder)
            {
                case "date_desc":
                    questions = questions.OrderBy(q => q.datumPitanja);
                    break;
                case "popularity":
                    questions = questions.OrderBy(q => q.brojLajkova);
                    break;
                case "popularity_desc":
                    questions = questions.OrderByDescending(q => q.brojLajkova);
                    break;
                default:
                    questions = questions.OrderByDescending(q => q.datumPitanja);
                    break;
            }

            return View(await questions.ToListAsync());
        }

        // GET: Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            
            var predmeti = await _context.Predmeti.ToListAsync();
            ViewBag.Predmeti = new SelectList(predmeti, "id", "naziv");
            
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("naslov,tekst,predmetId")] Pitanje pitanje, IFormFile? file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return Challenge();
                    }

                    pitanje.korisnikId = user.Id;
                    pitanje.datumPitanja = DateTime.Now;
                    pitanje.brojLajkova = 0;

                    // Handle file upload
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
                            
                            var predmeti = await _context.Predmeti.ToListAsync();
                            ViewBag.Predmeti = new SelectList(predmeti, "id", "naziv", pitanje.predmetId);
                            return View(pitanje);
                        }
                    }

                    _context.Add(pitanje);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Question posted successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while posting your question. Please try again.");
                
            }

            
            var predmetiList = await _context.Predmeti.ToListAsync();
            ViewBag.Predmeti = new SelectList(predmetiList, "id", "naziv", pitanje.predmetId);
            return View(pitanje);
        }

        // GET: Details
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pitanje = await _context.Pitanja
                .Include(p => p.autor)
                .Include(p => p.predmet)
                .Include(p => p.Odgovori)
                    .ThenInclude(o => o.korisnik)
                .FirstOrDefaultAsync(m => m.id == id);

            if (pitanje == null)
            {
                return NotFound();
            }

            
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    
                    ViewBag.HasLikedQuestion = await _context.PitanjeLajkovi
                        .AnyAsync(pl => pl.pitanjeId == id && pl.korisnikId == user.Id);

                    
                    var likedAnswers = await _context.OdgovorLajkovi
                        .Where(ol => ol.korisnikId == user.Id)
                        .Select(ol => ol.odgovorId)
                        .ToListAsync();
                    
                    ViewBag.LikedAnswers = likedAnswers;
                }
            }

            return View(pitanje);
        }

        // POST: AddAnswer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAnswer(int pitanjeId, [Bind("tekst")] Odgovor odgovor, IFormFile? file)
        {
            try
            {
                
                var pitanje = await _context.Pitanja.FindAsync(pitanjeId);
                if (pitanje == null)
                {
                    return NotFound();
                }

                if (string.IsNullOrWhiteSpace(odgovor.tekst))
                {
                    TempData["ErrorMessage"] = "Answer text is required.";
                    return RedirectToAction(nameof(Details), new { id = pitanjeId });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

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
                TempData["SuccessMessage"] = "Answer posted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while posting your answer. Please try again.";
                
            }

            return RedirectToAction(nameof(Details), new { id = pitanjeId });
        }

        // POST: LikeAnswer
        [HttpPost]
        public async Task<IActionResult> LikeAnswer(int answerId, int questionId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                var existingLike = await _context.OdgovorLajkovi
                    .FirstOrDefaultAsync(ol => ol.odgovorId == answerId && ol.korisnikId == user.Id);

                if (existingLike != null)
                {
                    // Ukloni lajk
                    _context.OdgovorLajkovi.Remove(existingLike);
                    
                    var odgovor = await _context.Odgovori.FindAsync(answerId);
                    if (odgovor != null)
                    {
                        odgovor.brojLajkova = Math.Max(0, odgovor.brojLajkova - 1);
                    }
                    
                    TempData["SuccessMessage"] = "Like removed!";
                }
                else
                {
                    // Dodaj lajk
                    var noviLajk = new OdgovorLajk
                    {
                        odgovorId = answerId,
                        korisnikId = user.Id
                    };
                    
                    _context.OdgovorLajkovi.Add(noviLajk);
                    
                    var odgovor = await _context.Odgovori.FindAsync(answerId);
                    if (odgovor != null)
                    {
                        odgovor.brojLajkova++;
                    }
                    
                    TempData["SuccessMessage"] = "Answer liked!";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = questionId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction(nameof(Details), new { id = questionId });
            }
        }

        // POST: LikeQuestion
        [HttpPost]
        public async Task<IActionResult> LikeQuestion(int pitanjeId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                var existingLike = await _context.PitanjeLajkovi
                    .FirstOrDefaultAsync(pl => pl.pitanjeId == pitanjeId && pl.korisnikId == user.Id);

                if (existingLike != null)
                {
                    // Ukloni lajk
                    _context.PitanjeLajkovi.Remove(existingLike);
                    
                    var pitanje = await _context.Pitanja.FindAsync(pitanjeId);
                    if (pitanje != null)
                    {
                        pitanje.brojLajkova = Math.Max(0, pitanje.brojLajkova - 1);
                    }
                    
                    TempData["SuccessMessage"] = "Like removed!";
                }
                else
                {
                    // Dodaj lajk
                    var noviLajk = new PitanjeLajk
                    {
                        pitanjeId = pitanjeId,
                        korisnikId = user.Id
                    };
                    
                    _context.PitanjeLajkovi.Add(noviLajk);
                    
                    var pitanje = await _context.Pitanja.FindAsync(pitanjeId);
                    if (pitanje != null)
                    {
                        pitanje.brojLajkova++;
                    }
                    
                    TempData["SuccessMessage"] = "Question liked!";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = pitanjeId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction(nameof(Details), new { id = pitanjeId });
            }
        }

        // Helper method for file upload
        private async Task<FileUploadResult> HandleFileUpload(IFormFile file)
        {
            try
            {
                // Check file size
                if (file.Length > _maxFileSize)
                {
                    return new FileUploadResult { Success = false, ErrorMessage = "File size exceeds 5MB limit." };
                }

                // Check file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new FileUploadResult { Success = false, ErrorMessage = "File type not allowed." };
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return new FileUploadResult
                {
                    Success = true,
                    FilePath = "/uploads/" + uniqueFileName,
                    OriginalFileName = file.FileName
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResult { Success = false, ErrorMessage = "An error occurred while uploading the file." };
            }
        }

        // Helper class for file upload results
        private class FileUploadResult
        {
            public bool Success { get; set; }
            public string? FilePath { get; set; }
            public string? OriginalFileName { get; set; }
            public string? ErrorMessage { get; set; }
        }
    }
}