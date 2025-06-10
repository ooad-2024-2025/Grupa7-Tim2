using System.ComponentModel.DataAnnotations;
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

        [Range(1, 5)]
        public int? ocjena {  get; set; }
        [MaxLength(500)]
        public string? komentar {  get; set; }

        public DateTime DatumUnosa { get; set; } = DateTime.Now;

    }
}
