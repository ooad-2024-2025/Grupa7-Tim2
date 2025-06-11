using ETForum.Data;
using ETForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETForum.Controllers
{
    [Authorize]
    public class DostignuceController : Controller
    {
        private readonly ETForumDbContext _context;

        public DostignuceController(ETForumDbContext context)
        {
            _context = context;
        }
        private bool DostignuceExists(int id)
        {
            return _context.Dostignuca.Any(e => e.id == id);
        }

        // GET: Dostignuce
        public async Task<IActionResult> Index()
        {
            var dostignuca = await _context.Dostignuca.ToListAsync();
            return View(dostignuca);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Dohvati dostignuće s id-jem
            var dostignuce = await _context.Dostignuca
                .FirstOrDefaultAsync(d => d.id == id);

            if (dostignuce == null)
            {
                return NotFound();  // Ako nije pronađeno, vrati 404
            }

            return View(dostignuce);  // Pošaljite model za uređivanje
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,naziv,opis,tip")] Dostignuce dostignuce)
        {
            if (id != dostignuce.id)
            {
                return NotFound();  // Provjeri ako id nije isti
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dostignuce);  // Ažuriraj model u bazi podataka
                    await _context.SaveChangesAsync();  // Spremi promjene
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DostignuceExists(dostignuce.id))  // Provjera postojanja dostignuća
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;  // Ako se dogodila konkurentnost
                    }
                }

                return RedirectToAction(nameof(Index));  // Preusmjerenje na indeksnu stranicu
            }

            return View(dostignuce);  // Ako nešto nije validno, ponovo prikaži formu
        }


        // GET: Moja dostignuća
        public async Task<IActionResult> MojaDostignuca()
        {
            var userId = User.Identity?.Name;
            if (userId == null) return RedirectToAction("Login", "Korisnik");

            var korisnik = await _context.Korisnici
                .Include(k => k.KorisnikDostignuca)
                    .ThenInclude(kd => kd.Dostignuce)
                .FirstOrDefaultAsync(k => k.UserName == userId);

            return View(korisnik);
        }

        // GET: Leaderboard
        public async Task<IActionResult> Leaderboard()
        {
            var korisnici = await _context.Korisnici
                .Include(k => k.KorisnikDostignuca)
                    .ThenInclude(kd => kd.Dostignuce)
                .ToListAsync();

            var rangLista = korisnici.Select(k => new
            {
                Korisnik = k,
                Bodovi = k.KorisnikDostignuca.Count()
            })
            .OrderByDescending(x => x.Bodovi)
            .Select((x, index) => new
            {
                Rang = index + 1,
                Korisnik = x.Korisnik,
                Bodovi = x.Bodovi
            })
            .ToList();

            return View(rangLista);
        }

        // GET: Dostignuce/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dostignuce/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("naziv,opis,tip")] Dostignuce dostignuce)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dostignuce);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dostignuce);
        }

        // GET: Dostignuce/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dostignuce = await _context.Dostignuca.FirstOrDefaultAsync(d => d.id == id);
            if (dostignuce == null) return NotFound();

            return View(dostignuce);
        }

        // POST: Dostignuce/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dostignuce = await _context.Dostignuca.FindAsync(id);
            if (dostignuce != null)
            {
                _context.Dostignuca.Remove(dostignuce);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
