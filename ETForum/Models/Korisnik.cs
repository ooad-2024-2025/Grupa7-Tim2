using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ETForum.Models

{
    public class Korisnik: IdentityUser
    {
        [Required]
        public string? ime {  get; set; }
        public string? prezime { get; set; }
        public string ImePrezime => ime + " " + prezime;
        public string? nickname { get; set; }
        public Uloga? uloga { get; set; }
        public DateTime datumRegistracije { get; set; }
        public bool status { get; set; }
        public Smjer? smjer { get; set; }
        public string? urlSlike { get; set; }
        public bool podesenProfil { get; set; }
        public DateTime? lastLogin { get; set; }

        public virtual ICollection<Dostignuce>? Dostignuca { get; set; }
    }
}
