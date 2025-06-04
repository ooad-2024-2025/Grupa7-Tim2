using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Dostignuce
    {
        public int id {  get; set; }
        public string? naziv {  get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? korisnik {  get; set; }
        public string? opis  { get; set; }
        public string? tip { get; set; }
    }
}
