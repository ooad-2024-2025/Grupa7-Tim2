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
    public class StudySessionsController : Controller
    {
        private readonly ETForumDbContext _context;

        public StudySessionsController(ETForumDbContext context)
        {
            _context = context;
        }

        // GET: StudySessions
        public async Task<IActionResult> Index()
        {
            var eTForumDbContext = _context.StudySession.Include(s => s.korisnik);
            return View(await eTForumDbContext.ToListAsync());
        }

        // GET: StudySessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studySession = await _context.StudySession
                .Include(s => s.korisnik)
                .FirstOrDefaultAsync(m => m.id == id);
            if (studySession == null)
            {
                return NotFound();
            }

            return View(studySession);
        }

        // GET: StudySessions/Create
        public IActionResult Create()
        {
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id");
            return View();
        }

        // POST: StudySessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,korisnikId,pocetak,kraj,trajanje")] StudySession studySession)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studySession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", studySession.korisnikId);
            return View(studySession);
        }

        // GET: StudySessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studySession = await _context.StudySession.FindAsync(id);
            if (studySession == null)
            {
                return NotFound();
            }
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", studySession.korisnikId);
            return View(studySession);
        }

        // POST: StudySessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,korisnikId,pocetak,kraj,trajanje")] StudySession studySession)
        {
            if (id != studySession.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studySession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudySessionExists(studySession.id))
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
            ViewData["korisnikId"] = new SelectList(_context.Korisnici, "Id", "Id", studySession.korisnikId);
            return View(studySession);
        }

        // GET: StudySessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studySession = await _context.StudySession
                .Include(s => s.korisnik)
                .FirstOrDefaultAsync(m => m.id == id);
            if (studySession == null)
            {
                return NotFound();
            }

            return View(studySession);
        }

        // POST: StudySessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studySession = await _context.StudySession.FindAsync(id);
            if (studySession != null)
            {
                _context.StudySession.Remove(studySession);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudySessionExists(int id)
        {
            return _context.StudySession.Any(e => e.id == id);
        }
    }
}
