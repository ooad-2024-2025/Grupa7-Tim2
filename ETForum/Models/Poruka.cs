using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Poruka
    {
        public int id { get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? korisnik { get; set; }
        public string? tekst {  get; set; }
        public DateTime datum {  get; set; }
        public bool procitana { get; set; }

    }
}
