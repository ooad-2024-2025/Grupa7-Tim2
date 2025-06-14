using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Dostignuce
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        public string naziv { get; set; }

        [Required(ErrorMessage = "Opis je obavezan.")]
        public string opis { get; set; }

        [Required(ErrorMessage = "Tip je obavezan.")]
        public TipDostignuca tip { get; set; }
        [BindNever]
        public ICollection<KorisnikDostignuce> KorisnikDostignuca { get; set; } = new List<KorisnikDostignuce>();

    }

    }
