using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Komentar
    {
        public int id { get; set; }
        public string? tekst { get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? autor { get; set; }
        public DateTime datumKreiranja { get; set; }
    }
}
