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
            var eTForumDbContext = _context.Dostignuca.Include(d => d.korisnik);
            return View(await eTForumDbContext.ToListAsync());
        }

        // GET: Dostignuce/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dostignuce = await _context.Dostignuca
                .Include(d => d.korisnik)
                .FirstOrDefaultAsync(m => m.id == id);
            if (dostignuce == null)
            {
                return NotFound();
            }

            return View(dostignuce);
        }

        // GET: Dostignuce/Create
        public IActionResult Create()
        {
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id");
            return View();
        }

        // POST: Dostignuce/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,naziv,korisnikId,opis,tip")] Dostignuce dostignuce)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dostignuce);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", dostignuce.korisnikId);
            return View(dostignuce);
        }

        // GET: Dostignuce/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dostignuce = await _context.Dostignuca.FindAsync(id);
            if (dostignuce == null)
            {
                return NotFound();
            }
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", dostignuce.korisnikId);
            return View(dostignuce);
        }

        // POST: Dostignuce/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,naziv,korisnikId,opis,tip")] Dostignuce dostignuce)
        {
            if (id != dostignuce.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dostignuce);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DostignuceExists(dostignuce.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", dostignuce.korisnikId);
            return View(dostignuce);
        }

        // GET: Dostignuce/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dostignuce = await _context.Dostignuca
                .Include(d => d.korisnik)
                .FirstOrDefaultAsync(m => m.id == id);
            if (dostignuce == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DostignuceExists(int id)
        {
            return _context.Dostignuca.Any(e => e.id == id);
        }
        */

        public async Task<IActionResult> Leaderboard()
        {
            var korisnici = await _context.Korisnici
                .Select(k => new
                {
                    Korisnik = k,
                    BrojLajkova = _context.Dostignuca.Count(d => d.korisnikId == k.Id && d.tip == TipDostignuca.Lajk),
                    BrojKomentara = _context.Dostignuca.Count(d => d.korisnikId == k.Id && d.tip == TipDostignuca.Komentar),
                    BrojPitanja = _context.Dostignuca.Count(d => d.korisnikId == k.Id && d.tip == TipDostignuca.Pitanje),
                    BrojOdgovora = _context.Dostignuca.Count(d => d.korisnikId == k.Id && d.tip == TipDostignuca.Odgovor),
                    BrojNajboljihOdgovora = _context.Dostignuca.Count(d => d.korisnikId == k.Id && d.tip == TipDostignuca.NajboljiOdgovor),
                    BrojPrijatelja = _context.Dostignuca.Count(d => d.korisnikId == k.Id && d.tip == TipDostignuca.Prijatelj)
                })
                .OrderByDescending(x => x.BrojLajkova)
                .ThenByDescending(x => x.BrojKomentara)
                .ThenByDescending(x => x.BrojPitanja)
                .ThenByDescending(x => x.BrojOdgovora)
                .ThenByDescending(x => x.BrojNajboljihOdgovora)
                .ThenByDescending(x => x.BrojPrijatelja)
                .ToListAsync();

            return View(korisnici);
        }
    }
}
