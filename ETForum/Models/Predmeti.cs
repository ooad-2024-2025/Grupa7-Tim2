using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class Predmeti
    {
        public int id { get; set; }
        public string? naziv { get; set; }
        public string? opis { get; set; }
        public string? profesorId { get; set; }
        [ForeignKey(nameof(profesorId))]
        public Korisnik? profesor { get; set; }
        public string? asistentId { get; set; }
        [ForeignKey(nameof(asistentId))]
        public Korisnik? asistent { get; set; }
    }
}