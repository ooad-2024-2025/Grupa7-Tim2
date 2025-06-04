using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ETForum.Models
{
    public class Pitanje
    {
        public int PitanjeID { get; set; }
        public string Naslov { get; set; }
        public string Opis { get; set; }
        public Date DatumPostavljanja { get; set; }
        public Smjer Kategorija { get; set; }
        public Korisnik Autor {  get; set; }
        public List<int> ListaOdgovora { get; set; }

    }
}
