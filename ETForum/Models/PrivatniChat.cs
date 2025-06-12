using System.ComponentModel.DataAnnotations.Schema;

namespace ETForum.Models
{
    public class PrivatniChat
    {
        public int Id { get; set; }

        public string? posiljalacId { get; set; }
        [ForeignKey(nameof(posiljalacId))]
        public Korisnik? posiljalac { get; set; }

        public string? primaocId { get; set; }
        [ForeignKey(nameof(primaocId))]
        public Korisnik? primaoc { get; set; }

        public string? poruka { get; set; }
        public DateTime vrijeme { get; set; }
        public bool procitano { get; set; } = false;
    }
}