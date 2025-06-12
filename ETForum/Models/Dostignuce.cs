using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Dostignuce
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string naziv { get; set; }
        public string opis { get; set; }
        public TipDostignuca tip { get; set; }
        public ICollection<KorisnikDostignuce> KorisnikDostignuca { get; set; }
    }

}
