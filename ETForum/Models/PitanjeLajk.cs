
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class PitanjeLajk
    {
        public int id { get; set; }

        public string korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik korisnik { get; set; }

        public int pitanjeId { get; set; }
        [ForeignKey(nameof(pitanjeId))]
        public Pitanje pitanje { get; set; }

        
    }
}