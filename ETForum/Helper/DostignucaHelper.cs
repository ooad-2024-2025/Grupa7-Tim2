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
                .Include(k => k.Dostignuca)
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
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Pitanje, "Prvo pitanje", "Postavi prvo pitanje", brojPitanja >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Pitanje, "5 pitanja", "Postavi ukupno 5 pitanja", brojPitanja >= 5);
        }

        private async Task ProvjeriOdgovore(Korisnik korisnik)
        {
            var brojOdgovora = await _context.Odgovori.CountAsync(o => o.korisnikId == korisnik.Id);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Odgovor, "Prvi odgovor", "Objavi svoj prvi odgovor", brojOdgovora >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Odgovor, "10 odgovora", "Objavi ukupno 10 odgovora", brojOdgovora >= 10);
        }

        private async Task ProvjeriKomentare(Korisnik korisnik)
        {
            var brojKomentara = await _context.Komentari.CountAsync(k => k.korisnikId == korisnik.Id);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Komentar, "Prvi komentar", "Dodaj komentar na sadržaj", brojKomentara >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Komentar, "5 komentara", "Dodaj ukupno 5 komentara", brojKomentara >= 5);

            var brojRazlicitih = await _context.Komentari.Where(k => k.korisnikId == korisnik.Id).Select(k => k.id).Distinct().CountAsync();
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Komentar, "Diskusija", "Napiši komentar na 10 različitih sadržaja", brojRazlicitih >= 10);
        }

        private async Task ProvjeriLajkove(Korisnik korisnik)
        {
            var brojLajkova = await _context.PitanjeLajkovi.CountAsync(p => p.korisnikId == korisnik.Id) +
                              await _context.OdgovorLajkovi.CountAsync(o => o.korisnikId == korisnik.Id);

            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Lajk, "Prvi lajk", "Lajkuj nešto prvi put", brojLajkova >= 1);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Lajk, "10 lajkova", "Osvoji ukupno 10 lajkova", brojLajkova >= 10);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Lajk, "Lajk-majstor", "Osvoji 50 lajkova", brojLajkova >= 50);
        }

        private async Task ProvjeriStudySession(Korisnik korisnik)
        {
            var prva = await _context.StudySession.AnyAsync(s => s.korisnikId == korisnik.Id);
            var maratonac = await _context.StudySession.AnyAsync(s => s.korisnikId == korisnik.Id && s.trajanje.HasValue && s.trajanje.Value.TotalMinutes >= 60);

            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Ostalo, "Dugme start", "Započni svoju prvu study sesiju", prva);
            await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Ostalo, "Maratonac", "Završi sesiju dužu od 60 minuta", maratonac);
        }

        private async Task ProvjeriPovratak(Korisnik korisnik)
        {
            var zadnjiLogin = korisnik.lastLogin;
            if (zadnjiLogin == null) return;

            var razlika = DateTime.Now - zadnjiLogin.Value;
            if (razlika.TotalDays >= 7)
            {
                await DodajDostignuceAkoNedostaje(korisnik, TipDostignuca.Ostalo, "Povratnik", "Vrati se na platformu nakon 7 dana pauze", true);
            }
        }

        private async Task DodajDostignuceAkoNedostaje(Korisnik korisnik, TipDostignuca tip, string naziv, string opis, bool uslov)
        {
            if (!uslov) return;
            if (korisnik.Dostignuca.Any(d => d.naziv == naziv)) return;

            var novo = new Dostignuce
            {
                korisnikId = korisnik.Id,
                naziv = naziv,
                opis = opis,
                tip = tip
            };

            _context.Dostignuca.Add(novo);
        }
    }
}
