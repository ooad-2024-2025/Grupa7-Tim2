using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class LiveChat
    {
        public int Id { get; set; }
        public string? korisnikId { get; set; }
        [ForeignKey(nameof(korisnikId))]
        public Korisnik? korisnik { get; set; }
        public string? username { get; set; }
        public string? poruka {  get; set; }
        public DateTime vrijeme {  get; set; }
    }
}
