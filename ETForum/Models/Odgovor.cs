using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Odgovor
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Tekst odgovora je obavezan")]
        [StringLength(2000, ErrorMessage = "Odgovor ne može biti duži od 2000 karaktera")]
        public string tekst { get; set; }

        public DateTime datumOdgovora { get; set; }
        public int brojLajkova { get; set; }

        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? korisnik { get; set; }

        public int pitanjeId { get; set; }
        [ForeignKey(nameof(pitanjeId))]
        public Pitanje? pitanje { get; set; }

        public ICollection<OdgovorLajk>? OdgovorLajkovi { get; set; }

        public string? FilePath { get; set; }
        public string? OriginalFileName { get; set; }
    }
}