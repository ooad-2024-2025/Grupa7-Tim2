using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Pitanje
    {
        public int id { get; set; }
        public string? tekst { get; set; }
        public DateTime datumPitanja { get; set; }
        public int brojLajkova { get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? autor { get; set; }
    }
}
