using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Prijateljstvo
    {
        public int id { get; set; }

        public string? korisnik1Id { get; set; } 
        public string? korisnik2Id { get; set; }  

        [ForeignKey(nameof(korisnik1Id))]
        public Korisnik? korisnik1 { get; set; }

        [ForeignKey(nameof(korisnik2Id))]
        public Korisnik? korisnik2 { get; set; }

        public Status status { get; set; } 
    }
}

