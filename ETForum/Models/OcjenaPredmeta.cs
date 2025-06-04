using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class OcjenaPredmeta
    {
        public int id {  get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? korisnik { get; set; }
        public int predmetiId { get; set; }
        [ForeignKey(nameof(predmetiId))]
        public Predmeti? predmet { get; set; }
        public int? ocjena {  get; set; }
        public string? komentar {  get; set; }

    }
}
