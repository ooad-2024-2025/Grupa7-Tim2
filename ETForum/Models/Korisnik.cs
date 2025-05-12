namespace ETForum.Models
{
    public class Korisnik
    {
        public int korisnikID {  get; set; }
        public string ime {  get; set; }
        public string prezime { get; set; }
        public string email { get; set; }
        public string lozinka { get; set; }
        public string nickname { get; set; }
        public Uloga? uloga { get; set; }
        public DateTime datumRegistracije { get; set; }
        public bool status { get; set; }
        public Smjer? smjer { get; set; }
    }
}
