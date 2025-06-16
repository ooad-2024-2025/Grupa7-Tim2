using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Notifikacija
    {
        public int Id { get; set; }
        public string KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        public int? pitanjeId { get; set; }
        [ForeignKey(nameof(pitanjeId))]
        public Pitanje Pitanje { get; set; }

        public string Tekst { get; set; }
        public string Link { get; set; }
        public bool Procitano { get; set; }
        public DateTime Vrijeme { get; set; }
    }

}
