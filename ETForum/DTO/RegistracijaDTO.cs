using System.ComponentModel.DataAnnotations;
using ETForum.Models;

namespace ETForum.DTO
{
    public class RegistracijaDTO
    {
        [Required]
        public string ime { get; set; } = string.Empty;
        [Required]
        public string prezime { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Lozinka mora imati između 8 i 20 karaktera.")]
        public string lozinka { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Nickname mora imati između 3 i 20 karaktera.")]
        public string nickname { get; set; } = string.Empty;
        [Required]
        public Uloga? uloga { get; set; }
        [Required]
        public Smjer? smjer { get; set; } 
    }
}
