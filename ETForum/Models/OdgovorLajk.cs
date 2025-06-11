using ETForum.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class OdgovorLajk
    {
        public int id { get; set; }

        public string korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik korisnik { get; set; }

        public int odgovorId { get; set; }
        [ForeignKey(nameof(odgovorId))]
        public Odgovor odgovor { get; set; }


    }
}