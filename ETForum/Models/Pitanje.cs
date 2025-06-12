using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Pitanje
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Naslov je obavezan")]
        [StringLength(200, ErrorMessage = "Naslov ne može biti duži od 200 karaktera")]
        public string naslov { get; set; }

        [Required(ErrorMessage = "Tekst pitanja je obavezan")]
        [StringLength(2000, ErrorMessage = "Tekst ne može biti duži od 2000 karaktera")]
        public string tekst { get; set; }

        public DateTime datumPitanja { get; set; }
        public int brojLajkova { get; set; }

        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? autor { get; set; }

        public int? predmetId { get; set; }
        [ForeignKey(nameof(predmetId))]
        public Predmeti? predmet { get; set; }

        public ICollection<Odgovor>? Odgovori { get; set; }
        public ICollection<PitanjeLajk>? PitanjeLajkovi { get; set; }

        public string? FilePath { get; set; }
        public string? OriginalFileName { get; set; }
    }
}