using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ETForum.Data;
using ETForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ETForum.Controllers
{
    [Authorize]
    public class StudySessionsController : Controller
    {
        private readonly ETForumDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public StudySessionsController(ETForumDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: StudySessions
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var sessions = await _context.StudySession
                .Where(s => s.korisnikId == user.Id)
                .OrderByDescending(s => s.pocetak)
                .ToListAsync();

            var postojiAktivna = sessions.Any(s => s.kraj == null);
            ViewBag.AktivnaSesija = postojiAktivna;

            return View(sessions);
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
        [Authorize]
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartSession()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            bool postojiAktivna = await _context.StudySession
                .AnyAsync(s => s.korisnikId == user.Id && s.kraj == null);

            if (postojiAktivna)
            {
                TempData["Poruka"] = "Već imate aktivnu sesiju. Završite je pre nego što započnete novu.";
                return RedirectToAction("Index");
            }

            var session = new StudySession
            {
                korisnikId = user.Id,
                pocetak = DateTime.Now
            };

            _context.StudySession.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndSession(int id)
        {
            var session = await _context.StudySession.FirstOrDefaultAsync(s => s.id == id);
            if (session == null || session.kraj != null) return NotFound();

            session.kraj = DateTime.Now;
            session.trajanje = session.kraj - session.pocetak;

            _context.Update(session);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }

}
