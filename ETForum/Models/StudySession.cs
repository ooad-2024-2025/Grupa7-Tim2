using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class StudySession
    {
        [Key] public int id { get; set; }
        [ForeignKey("Korisnik")] public string? korisnikId { get; set; }
        public Korisnik? korisnik { get; set; }
        public DateTime? pocetak { get; set; }
        public DateTime? kraj { get; set; }
        public TimeSpan? trajanje { get; set; }
        public int? predmetId { get; set; }
        [ForeignKey(nameof(predmetId))]
        public Predmeti? predmet { get; set; }

    }
}

