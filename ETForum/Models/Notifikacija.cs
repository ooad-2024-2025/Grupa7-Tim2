using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Notifikacija
    {
        public int id { get; set; }
        public int porukaId { get; set; }
        [ForeignKey(nameof(porukaId))]
        public Poruka? poruka { get; set; }
        public DateTime vrijemeKreiranja { get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? korisnik { get; set; }
        public bool procitana { get; set; }

    }
}
