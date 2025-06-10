using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ETForum.Data;
using ETForum.Models;

namespace ETForum.Controllers
{
    public class PredmetiController : Controller
    {
        private readonly ETForumDbContext _context;

        public PredmetiController(ETForumDbContext context)
        {
            _context = context;
        }

        // GET: Predmeti
        public async Task<IActionResult> Index()
        {
            return View(await _context.Predmeti.ToListAsync());
        }

        // GET: Predmeti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var predmeti = await _context.Predmeti
                .FirstOrDefaultAsync(m => m.id == id);
            if (predmeti == null)
            {
                return NotFound();
            }

            return View(predmeti);
        }

        // GET: Predmeti/Create
        public IActionResult Create()
        {
            ViewBag.Smjer = new SelectList(Enum.GetValues(typeof(Smjer)));
            return View();
        }

        // POST: Predmeti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,naziv,opis,Smjer,profesorImePrezime,asistentImePrezime")] Predmeti predmeti)
        {
            if (ModelState.IsValid)
            {
                _context.Add(predmeti);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(predmeti);
        }

        // GET: Predmeti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var predmeti = await _context.Predmeti.FindAsync(id);
            if (predmeti == null)
            {
                return NotFound();
            }

            ViewBag.Smjer = new SelectList(Enum.GetValues(typeof(Smjer)), predmeti.Smjer);
            return View(predmeti);
        }

        // POST: Predmeti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,naziv,opis,Smjer,profesorImePrezime,asistentImePrezime")] Predmeti predmeti)
        {
            if (id != predmeti.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(predmeti);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PredmetiExists(predmeti.id))
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
            return View(predmeti);
        }

        // GET: Predmeti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var predmeti = await _context.Predmeti
                .FirstOrDefaultAsync(m => m.id == id);
            if (predmeti == null)
            {
                return NotFound();
            }

            return View(predmeti);
        }

        // POST: Predmeti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var predmeti = await _context.Predmeti.FindAsync(id);
            if (predmeti != null)
            {
                _context.Predmeti.Remove(predmeti);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PredmetiExists(int id)
        {
            return _context.Predmeti.Any(e => e.id == id);
        }
    }
}
