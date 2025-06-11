using ETForum.Data;
using ETForum.Models;
using Microsoft.EntityFrameworkCore;

namespace ETForum.Helper
{
    public class DostignucaHelper
    {
        private readonly ETForumDbContext _context;

        public DostignucaHelper(ETForumDbContext context)
        {
            _context = context;
        }

        public async Task ProvjeriDostignucaZaKorisnika(string korisnikId)
        {
            var korisnik = await _context.Korisnici
                .Include(k => k.KorisnikDostignuca)
                .ThenInclude(kd => kd.Dostignuce)
                .FirstOrDefaultAsync(k => k.Id == korisnikId);

            if (korisnik == null) return;

            await ProvjeriPitanja(korisnik);
            await ProvjeriOdgovore(korisnik);
            await ProvjeriKomentare(korisnik);
            await ProvjeriLajkove(korisnik);
            await ProvjeriStudySession(korisnik);
            await ProvjeriPovratak(korisnik);

            await _context.SaveChangesAsync();
        }

        private async Task ProvjeriPitanja(Korisnik korisnik)
        {
            var brojPitanja = await _context.Pitanja.CountAsync(p => p.korisnikId == korisnik.Id);
            await DodajDostignuceAkoNedostaje(korisnik, "Prvo pitanje", () => brojPitanja >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, "5 pitanja", () => brojPitanja >= 5);
        }

        private async Task ProvjeriOdgovore(Korisnik korisnik)
        {
            var brojOdgovora = await _context.Odgovori.CountAsync(o => o.korisnikId == korisnik.Id);
            await DodajDostignuceAkoNedostaje(korisnik, "Prvi odgovor", () => brojOdgovora >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, "10 odgovora", () => brojOdgovora >= 10);
        }

        private async Task ProvjeriKomentare(Korisnik korisnik)
        {
            var brojKomentara = await _context.Komentari.CountAsync(k => k.korisnikId == korisnik.Id);
            await DodajDostignuceAkoNedostaje(korisnik, "Prvi komentar", () => brojKomentara >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, "5 komentara", () => brojKomentara >= 5);

            var brojRazlicitih = await _context.Komentari.Where(k => k.korisnikId == korisnik.Id).Select(k => k.id).Distinct().CountAsync();
            await DodajDostignuceAkoNedostaje(korisnik, "Diskusija", () => brojRazlicitih >= 10);
        }

        private async Task ProvjeriLajkove(Korisnik korisnik)
        {
            var brojLajkova = await _context.PitanjeLajkovi.CountAsync(p => p.korisnikId == korisnik.Id) +
                              await _context.OdgovorLajkovi.CountAsync(o => o.korisnikId == korisnik.Id);

            await DodajDostignuceAkoNedostaje(korisnik, "Prvi lajk", () => brojLajkova >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, "10 lajkova", () => brojLajkova >= 10);
            await DodajDostignuceAkoNedostaje(korisnik, "Lajk-majstor", () => brojLajkova >= 50);
        }

        private async Task ProvjeriStudySession(Korisnik korisnik)
        {

            var studySessions = await _context.StudySession
                .Where(s => s.korisnikId == korisnik.Id)
                .ToListAsync();  

            var prva = studySessions.Any();

            // Provjeri ako postoji sesija koja traje više od 60 minuta
            var maratonac = studySessions.Any(s => s.trajanje.HasValue && s.trajanje.Value.TotalMinutes >= 60);

            // Dodaj dostignuća na osnovu tih provjera
            await DodajDostignuceAkoNedostaje(korisnik, "Dugme start", () => prva);
            await DodajDostignuceAkoNedostaje(korisnik, "Maratonac", () => maratonac);
        }


        private async Task ProvjeriPovratak(Korisnik korisnik)
        {
            var zadnjiLogin = korisnik.lastLogin;
            if (zadnjiLogin == null) return;

            var razlika = DateTime.Now - zadnjiLogin.Value;
            await DodajDostignuceAkoNedostaje(korisnik, "Povratnik", () => razlika.TotalDays >= 7);
        }

        private async Task DodajDostignuceAkoNedostaje(Korisnik korisnik, string naziv, Func<bool> uslov)
        {
            var dostignuce = await _context.Dostignuca.FirstOrDefaultAsync(d => d.naziv == naziv);
            if (dostignuce == null || !uslov()) return;

            bool vecIma = await _context.KorisnikDostignuca
                .AnyAsync(kd => kd.korisnikId == korisnik.Id && kd.dostignuceId == dostignuce.id);

            if (vecIma) return;

            var novo = new KorisnikDostignuce
            {
                korisnikId = korisnik.Id,
                dostignuceId = dostignuce.id
            };

            _context.KorisnikDostignuca.Add(novo);
        }

    }
}
