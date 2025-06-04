using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class StudySession
    {
        public int id {  get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? korisnik { get; set; }
        public DateTime? pocetak {  get; set; }
        public DateTime? kraj {  get; set; }
        public TimeSpan? trajanje { get; set; }
    }
}
