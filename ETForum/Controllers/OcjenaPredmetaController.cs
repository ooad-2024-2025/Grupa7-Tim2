using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ETForum.Data;
using ETForum.Models;
using System.Security.Claims;

namespace ETForum.Controllers
{
    public class OcjenaPredmetaController : Controller
    {
        private readonly ETForumDbContext _context;

        public OcjenaPredmetaController(ETForumDbContext context)
        {
            _context = context;
        }
        /*
        // GET: OcjenaPredmeta
        public async Task<IActionResult> Index()
        {
            var eTForumDbContext = _context.OcjenaPredmeta.Include(o => o.korisnik).Include(o => o.predmet);
            return View(await eTForumDbContext.ToListAsync());
        }

        // GET: OcjenaPredmeta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocjenaPredmeta = await _context.OcjenaPredmeta
                .Include(o => o.korisnik)
                .Include(o => o.predmet)
                .FirstOrDefaultAsync(m => m.id == id);
            if (ocjenaPredmeta == null)
            {
                return NotFound();
            }

            return View(ocjenaPredmeta);
        }

        // GET: OcjenaPredmeta/Create
        public IActionResult Create()
        {
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id");
            ViewData["predmetiId"] = new SelectList(_context.Predmeti, "id", "id");
            return View();
        }

        // POST: OcjenaPredmeta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,korisnikId,predmetiId,ocjena,komentar,DatumUnosa")] OcjenaPredmeta ocjenaPredmeta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ocjenaPredmeta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", ocjenaPredmeta.korisnikId);
            ViewData["predmetiId"] = new SelectList(_context.Predmeti, "id", "id", ocjenaPredmeta.predmetiId);
            return View(ocjenaPredmeta);
        }

        // GET: OcjenaPredmeta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocjenaPredmeta = await _context.OcjenaPredmeta.FindAsync(id);
            if (ocjenaPredmeta == null)
            {
                return NotFound();
            }
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", ocjenaPredmeta.korisnikId);
            ViewData["predmetiId"] = new SelectList(_context.Predmeti, "id", "id", ocjenaPredmeta.predmetiId);
            return View(ocjenaPredmeta);
        }

        // POST: OcjenaPredmeta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,korisnikId,predmetiId,ocjena,komentar,DatumUnosa")] OcjenaPredmeta ocjenaPredmeta)
        {
            if (id != ocjenaPredmeta.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ocjenaPredmeta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OcjenaPredmetaExists(ocjenaPredmeta.id))
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
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", ocjenaPredmeta.korisnikId);
            ViewData["predmetiId"] = new SelectList(_context.Predmeti, "id", "id", ocjenaPredmeta.predmetiId);
            return View(ocjenaPredmeta);
        }

        // GET: OcjenaPredmeta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ocjenaPredmeta = await _context.OcjenaPredmeta
                .Include(o => o.korisnik)
                .Include(o => o.predmet)
                .FirstOrDefaultAsync(m => m.id == id);
            if (ocjenaPredmeta == null)
            {
                return NotFound();
            }

            return View(ocjenaPredmeta);
        }

        // POST: OcjenaPredmeta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ocjenaPredmeta = await _context.OcjenaPredmeta.FindAsync(id);
            if (ocjenaPredmeta != null)
            {
                _context.OcjenaPredmeta.Remove(ocjenaPredmeta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OcjenaPredmetaExists(int id)
        {
            return _context.OcjenaPredmeta.Any(e => e.id == id);
        }
        */

        // 1. Izbor smjera
        public IActionResult Smjer()
        {
            var smjer = Enum.GetValues(typeof(Smjer)).Cast<Smjer>();
            return View(smjer);
        }

        // 2. Prikaz predmeta za izabrani smjer
        public IActionResult PredmetiZaSmjer(Smjer smjer)
        {
            var predmeti = _context.Predmeti
                .Where(p => p.Smjer == smjer)
                .ToList();
            ViewBag.Smjer = smjer;
            return View(predmeti);
        }

        // 3. Detalji predmeta + komentari i prosjek
        public IActionResult DetaljiPredmeta(int predmetId)
        {
            var predmet = _context.Predmeti
               
                .FirstOrDefault(p => p.id == predmetId);

            var ocjene = _context.OcjenaPredmeta
                .Where(o => o.predmetiId == predmetId)
                .OrderByDescending(o => o.DatumUnosa)
                .ToList();

            ViewBag.Prosjek = ocjene.Any() ? ocjene.Average(o => o.ocjena ?? 0) : 0;
            ViewBag.Ocjene = ocjene;

            return View(predmet);
        }

        // 4. GET: Forma za ocjenjivanje (prikaz)
        public IActionResult Ocijeni(int predmetId)
        {
            var predmet = _context.Predmeti.FirstOrDefault(p => p.id == predmetId);
            if (predmet == null)
                return NotFound();

            ViewBag.PredmetId = predmetId;
            ViewBag.NazivPredmeta = predmet.naziv;
            return View();
        }

        // 5. POST: Slanje ocjene i komentara
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ocijeni(int predmetId, int ocjena, string komentar)
        {
            // Možeš koristiti ID logovanog korisnika, ili ostaviti null za anonimno
            string? korisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var novaOcjena = new OcjenaPredmeta
            {
                predmetiId = predmetId,
                ocjena = ocjena,
                komentar = komentar,
                korisnikId = korisnikId,
                DatumUnosa = DateTime.Now
            };

            _context.OcjenaPredmeta.Add(novaOcjena);
            _context.SaveChanges();

            return RedirectToAction("DetaljiPredmeta", new { predmetId });
        }
        public IActionResult RI(string smjer)
        {
            ViewData["Smjer"] = smjer;
            return View();
        }
        public IActionResult TK(string smjer)
        {
            ViewData["Smjer"] = smjer;
            return View();
        }
        public IActionResult EE(string smjer)
        {
            ViewData["Smjer"] = smjer;
            return View();
        }
        public IActionResult Smjerovi(string smjer)
        {
            ViewData["Smjer"] = smjer;
            return View();
        }

    }
}
