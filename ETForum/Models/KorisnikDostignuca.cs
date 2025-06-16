using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class KorisnikDostignuce
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik Korisnik { get; set; }
        [Required]
        public int dostignuceId { get; set; }
        [ForeignKey(nameof(dostignuceId))]
        public Dostignuce Dostignuce { get; set; }
    }
}
